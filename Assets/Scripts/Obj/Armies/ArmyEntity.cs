using System.Collections;
using static System.Array;
using System.Collections.Generic;
using UnityEngine;
using HistoryHex;

public class ArmyEntity : MonoBehaviour
{
    #region Properties
    // Internal variables
    private bool activated = false;
    private bool hasMoved = false;
    public Vector3Int Position;
    public float Food;

	private int _manpower;
    public int Manpower{
		get{
			return _manpower;
		}
		set{
			int amountLost = _manpower - value;
			_manpower = value;
			if (amountLost >= 0){
				DisplayDmg(amountLost, transform.position, new Vector3(0, 2.5f, 0), 2);
			}
		}
	}
    public string Name;
    public Player Controller;
    GameObject pathObject;

    // UI_COMponents.
    public GameObject UIComponentPrefab; // UIComponentPrefab
    private GameObject UIComponentInstance;

    // SelectionInterface
    private SelectableObj SelectionInterface;
    public EntityDrawer drawer;

    //Current Action Mode.
    public ArmyActionMode ActionMode;

    //DiceRoller 
    private DiceRoller roller;
    public int PowerPerRoll = 100;
    public int PowerPerDamage = 20;
    public int MaxOffenseRolls = 3;
    public int MaxDefenseRolls = 2;
    public List<HexPath> supplyLines;
    #endregion

    #region MonobehaivorExtensions
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        MapDrawingUpdater();

        //If Activated, run the extended activation methods.
        if (activated && SelectedByController())
        {
            ActiveUpdate();
        }

        //Draw the Entity.
        Draw();

        if (Input.GetKeyDown(KeyCode.P))
        {
            CreateSprites();
        }
    }

    // Remove the Event Listener. May no longer be required, but not sure.
    private void OnDestroy()
    {
        Destroy(SelectionInterface);
        Destroy(pathObject);
        Global.GM.ArmyUpdate -= OnStartTurn;

    }

    void Initialize()
    {
        supplyLines = new List<HexPath>();
        Name = "Army";
        Food = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);
        Manpower = 100;
        roller = new DiceRoller(6);
        // Create a drawer.
        drawer = new EntityDrawer(transform);

        //Attempt to wire the SelectionInterface.
        SelectionInterface = transform.GetComponent<SelectableObj>();
        if (SelectionInterface == null)
        {
            throw new UnityException("Failed to link Army Entity to a SelectionInterface.");
        }
        else
        {
            WireSelectionInterface();
        }

        //Wire up the GM
        Global.GM.ArmyUpdate += OnStartTurn;

        //Present UI Components.

        // Create sprites
        CreateSprites();
    }

    private void ActiveUpdate()
    {
        //Check for the Mode Change key.
        if (Input.GetKeyDown(KeyCode.M))
        {
            ActionMode = ArmyActionMode.Move;
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            ActionMode = ArmyActionMode.SetSupply;
        }

        return;
    }

    private void MapDrawingUpdater()
    {
        // Add a food mapping at some point.
        // Shows the Control Map.
        if (Global.CurrentMapMode == MapMode.Controller)
        {
            if (Controller != null)
            {
                drawer.Color = Controller.Colour;
            }
            else
            {
                drawer.Color = Color.black;
            }
        }
    }

    #endregion

    #region Unit Actions

    /// <summary>
    /// Moves the unit across the board relative to current position.
    /// </summary>
    public void MoveAction(Vector3Int direction)
    {
        //Destroy(pathObject);
        Vector3 moveTo = Global.GetCubicVector(direction.x, direction.y, direction.z);
        Vector3Int nextPos = new Vector3Int(Position.x + direction.x, Position.y + direction.y, Position.z + direction.z);
        if (Global.MapFlyWeight.HasHexAtCubic(nextPos))
        {
            //Get the tile for any operations that might be necessary.
            GameObject nextTile = Global.MapFlyWeight.hexMap[nextPos];
            GameObject currentTile = Global.MapFlyWeight.hexMap[Position];
            HexEntity nextHexEntity = nextTile.GetComponent<HexEntity>();
            HexEntity currentHexEntity = currentTile.GetComponent<HexEntity>();

			if (nextHexEntity.army != null && nextHexEntity.army.GetComponent<ArmyEntity>().Controller.PlayerId == Controller.PlayerId){
				return;
			}

            if (nextHexEntity.army == null)
            {
                currentHexEntity.army = null;

                Sieze(nextTile);
                transform.Translate(moveTo);
                Position = nextPos;
                RefreshSupplyLines();
            }
            else
            {
                Combat(nextHexEntity.army);
                if (nextHexEntity.army == null)
                {
                    currentHexEntity.army = null;
                    Sieze(nextTile);
                    transform.Translate(moveTo);
                    Position = nextPos;
                    RefreshSupplyLines();
                }
            }
        }
    }

    /// <summary>
    /// Adds a supply line from this tile to a baseTile.
    /// </summary>
    /// <param name="baseTile">Tile to link a supply line to.</param>
    public void AddSupplyLine(GameObject baseTile)
    {
        //Add logic to check what the other is, and attempt to create a path to that location.
        HexEntity hex = baseTile.GetComponent<HexEntity>();
        if (hex == null)
        {
            // do nothing.
        }
        else
        {
            //create a path between this tile and that one.
            if (pathObject != null)
            {
                Destroy(pathObject);
                pathObject = null;
            }
            GameObject armyTile = Global.MapFlyWeight.hexMap[Position];

            pathObject = Instantiate(Global.MapFlyWeight.hexPathPrefab);
            HexPath path = pathObject.GetComponent<HexPath>();
            path.Initialize();
			path.army = this;

            List<GameObject> hexes = Global.MapFlyWeight.getPlayerAdjacencyMap(this.Controller).NearestAstar(armyTile, baseTile);
            path.AddHexes(hexes);
            supplyLines.Add(path);

			path.RegisterOrder();
        }
    }

    public void RefreshSupplyLines()
    {
        if (pathObject == null)
            return;
        HexPath path = pathObject.GetComponent<HexPath>();
        if (path == null)
            return;
        GameObject armyTile = Global.GM.Board.hexMap[Position];
        List<GameObject> hexes = Global.GM.Board.getPlayerAdjacencyMap(this.Controller).NearestAstar(armyTile, path.GetHex(path.Length() - 1));
        path.Refresh(hexes);
    }

    /// <summary>
    /// Moves this army to another tile.
    /// </summary>
    /// <param name="targetTile"></param>
    public void Move(GameObject targetTile)
    {
        // Add logic to check what the other is, and attempt to create a path to that location.
        HexEntity TargetHex = targetTile.GetComponent<HexEntity>();

        // Verify the action should be taken.
        // Did you know C# thinks & has higher priority then | ? Rediculous!
        if (TargetHex == null || hasMoved)
        {
            // Do nothing.
        }
        else
        {
            // checks to see if the tile can be moved to.
            HexEntity myHex = Global.MapFlyWeight.hexMap[Position].GetComponent<HexEntity>();
            if (TargetHex.Adjacent(myHex))
            {
                Vector3Int direction = TargetHex.Position - myHex.Position;
                MoveAction(direction);
                hasMoved = true;
            }
        }
    }
    /// <summary>
    /// Moves into a tile, and takes control. Assumes tile is unoccupied.
    /// </summary>
    /// <param name="hexTile"></param>
    public void Sieze(GameObject hexTile)
    {
        HexEntity entity = hexTile.GetComponent<HexEntity>();
        Global.MapFlyWeight.TransferHexOwner(hexTile, Controller);
		entity.UpdateController(Controller);
        entity.army = gameObject;

		//Play the movement sound
		GameObject AudioObj = GameObject.Find("AudioManagerGame");
		ArbitrarySounds sounds = AudioObj.GetComponentInChildren<ArbitrarySounds>();
		sounds.OnArmyMove();
    }

    /// <summary>
    /// Combats another unit. Return true if winning combat, false otherwise
    /// </summary>
    public bool Combat(GameObject otherArmyObject)
    {
        if (otherArmyObject != null)
        {
            ArmyEntity otherArmy = otherArmyObject.GetComponent<ArmyEntity>();
            List<int> myRolls = ArmyRoll(true);
            List<int> theirRolls = otherArmy.ArmyRoll(false);
            Debug.Log("My rolls " + myRolls);
            Debug.Log("And theirs " + myRolls);
            //DisplayRolls(myRolls, transform.position, new Vector3(0, 3, 0), 2);
            //DisplayRolls(theirRolls, otherArmyObject.transform.position, new Vector3(0, 3, 0), 2);

            int myDamage = 0, theirDamage = 0;
            for (int i = 0; i < theirRolls.Count && i < myRolls.Count; i++)
            {
                if (myRolls[i] <= theirRolls[i])
                    myDamage++;
                if (myRolls[i] >= theirRolls[i])
                    theirDamage++;
            }
			
			myDamage *= PowerPerDamage;
			theirDamage *= PowerPerDamage;
			//DisplayDmg(myDamage, transform.position, new Vector3(0, 2.5f, 0), 2);
			//DisplayDmg(theirDamage, otherArmyObject.transform.position, new Vector3(0, 2.5f, 0), 2);

			Manpower -= myDamage;
            otherArmy.Manpower -= PowerPerDamage;
            CheckDead();
            otherArmy.CheckDead();
        }

		//Play the movement sound
		GameObject AudioObj = GameObject.Find("AudioManagerGame");
		ArbitrarySounds sounds = AudioObj.GetComponentInChildren<ArbitrarySounds>();
		sounds.OnArmyAttack();

		return otherArmyObject == null;
    }
    private void DisplayRolls(List<int> rolls, Vector3 position, Vector3 offset, float lifespan)
    {
        GameObject textObject = new GameObject();
        TextMesh textComponent = textObject.AddComponent<TextMesh>();
        textObject.transform.position = position + offset;
		textObject.transform.localScale = new Vector3(0.5f,0.5f,0.5f);

		string text = "";
        foreach (int roll in rolls)
        {
            text += roll + " ";
        }
        textComponent.text = text;
        textComponent.color = Color.red;
        textComponent.fontSize = 10;
        Destroy(textObject, lifespan);
    }

	private void DisplayDmg(int amount, Vector3 position, Vector3 offset, float lifespan) {
		GameObject textObject = new GameObject();
		TextMesh textComponent = textObject.AddComponent<TextMesh>();
		textObject.transform.position = position + offset;
		textObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

		textComponent.text = "- "+ amount.ToString();
		textComponent.color = Color.red;
		textComponent.fontSize = 20;
		Destroy(textObject, lifespan);
	}
	public void CheckDead()
    {
        // Death when no manpower remaining.
        if (Manpower <= 0)
        {
            Destroy(gameObject);
            Global.MapFlyWeight.hexMap[Position].GetComponent<HexEntity>().army = null;
            Controller.armies.Remove(this);
        }
    }

    public List<int> ArmyRoll(bool offense)
    {
        List<int> rolls = roller.Roll(GetNumberRolls(offense));
        rolls.Sort();
        return rolls;
    }

    private int GetNumberRolls(bool offense)
    {
        float n = Mathf.Floor(Mathf.Max(1f, Manpower / PowerPerRoll));
        if (offense)
            return (int)Mathf.Min(n, MaxOffenseRolls);
        return (int)Mathf.Min(n, MaxDefenseRolls);
    }

    /// <summary>
    /// Attempt to pull food from the tile.
    /// Further testing required.
    /// </summary>
    private void ForageTile(int amount)
    {

        int collected = Global.MapFlyWeight.hexMap[Position].GetComponent<HexEntity>().FoodRequest(amount,0); //Damn this is long.
        if (collected < amount)
        {
            if (pathObject != null)
            {
                collected += pathObject.GetComponent<HexPath>().FoodRequest(amount - collected);
            }
        }

        Food += collected;
    }

    #endregion
	
    #region WireSelectionInterface

    /// <summary>
    /// Does everything needed to update the army at the start of the turn.
    /// </summary>
    private void OnStartTurn()
    {
        RefreshSupplyLines();

        //Set has moved to false.
        hasMoved = false;

        // Forage for food. Currently tries to get enough rations for just the current army.
        ForageTile(Manpower);
        Food -= Manpower;

        // Starvation mechanic
        if (Food < 0)
        {
            Manpower += Mathf.FloorToInt(Food);
            Food = 0;
        }

        CheckDead();

        Debug.Log("Current Food: " + Food);
        Debug.Log("Current Manpower: " + Manpower);
    }

    /// <summary>
    /// Wires up all the event handlers for the this entity.
    /// </summary>
    private void WireSelectionInterface()
    {
        SelectionInterface.Prepare();
        SelectionInterface.OnSelect += OnSelect;
        SelectionInterface.OnDeselect += OnDeselect;
        SelectionInterface.OnRightClick += OnRightClick;
        SelectionInterface.OnInitializeUI += OnInitializeUI;
    }

    private void OnSelect()
    {
        activated = true;

        //update army ui
        foreach (UnitSprite unitSprite in currentSpriteSlotHolder.GetComponentsInChildren<UnitSprite>())
        {
            unitSprite.StartPosing();
        }
        //pass function to be executed on 
    }

    private void OnDeselect()
    {
        Debug.Log("Deselect army entitiy");
        activated = false;
		ActionMode = ArmyActionMode.Move;

        foreach (UnitSprite unitSprite in currentSpriteSlotHolder.GetComponentsInChildren<UnitSprite>())
        {
            unitSprite.StopPosing();
        }
    }

    /// <summary>
    /// What to do when something has been right clicked.
    /// </summary>
    /// <param name="other">The object that has been picked with a raycast.</param>
    private void OnRightClick(GameObject other)
    {
        //Depending on the mod, army will do a different action.
        if (activated && SelectedByController())
        {
            if (ActionMode == ArmyActionMode.Move)
            {
                Move(other);
            }
            else if (ActionMode == ArmyActionMode.SetSupply)
            {
                AddSupplyLine(other);
            }
        }
    }

    private void OnInitializeUI(UICom com)
    {
        UIArmy uiArmy = (UIArmy)com;
        uiArmy.SetText(Name, (Controller.PlayerId + 1).ToString(), Food.ToString(), "", Manpower.ToString(), "", "", "");
        void ArmyMove() { ActionMode = ArmyActionMode.Move; }
        void ArmySupply() { ActionMode = ArmyActionMode.SetSupply; }
        uiArmy.SetButtonListeners(ArmyMove, ArmySupply);
    }

    #endregion

    #region SpriteRepresentation
    public Transform singleSpriteSlotHolder;
    public Transform doubleSpriteSlotHolder;
    public Transform tripleSpriteSlotHolder;
    public GameObject unitSpritePrefab;
    Transform currentSpriteSlotHolder;

    public Material team1Mat;
    public Material team2Mat;
    
    public void CreateSprites()
    {
        int oldNumSprites = 0;

        if (currentSpriteSlotHolder != null)
        {
            oldNumSprites = currentSpriteSlotHolder.childCount;
        }

        int requiredSprites = 3;

        Transform nextSpritesHolder = tripleSpriteSlotHolder;
        if (Manpower < 33)
        {
            requiredSprites = 1;
            nextSpritesHolder = singleSpriteSlotHolder;
        }
        else if (Manpower < 66)
        {
            requiredSprites = 2;
            nextSpritesHolder = doubleSpriteSlotHolder;
        }

        int spritesPlaced = 0;

        for (int i = 0; i < oldNumSprites; i++)
        {
            if (spritesPlaced < requiredSprites)
            {
                // move the old sprite to the new slot
                currentSpriteSlotHolder.GetChild(spritesPlaced).GetChild(0).SetParent(nextSpritesHolder.GetChild(spritesPlaced));
                spritesPlaced++;
            }
        }

        // instantiate remaining sprites
        for (; spritesPlaced < requiredSprites; spritesPlaced++)
        {
            GameObject newUnitSprite = GameObject.Instantiate(unitSpritePrefab, nextSpritesHolder.GetChild(spritesPlaced));
            newUnitSprite.GetComponent<SpriteRenderer>().material = Controller.PlayerId == 0 ? team1Mat : team2Mat;
        }

        if (spritesPlaced < oldNumSprites)
        {
            List<Transform> spritesToDestroy = new List<Transform>();
            for (int i = spritesPlaced; i < oldNumSprites; i++)
            {
                spritesToDestroy.Add(currentSpriteSlotHolder.GetChild(i).GetChild(0));
            }
            for (int i = 0; i < spritesToDestroy.Count; i++)
            {
                Destroy(spritesToDestroy[i].gameObject);
            }
        }

        currentSpriteSlotHolder = nextSpritesHolder;

        ChangeFacingDirection(Random.Range(0, 2) == 0);
    }

    void ChangeFacingDirection(bool faceLeft)
    {
        float facingDir = faceLeft ? -1 : 1;

        foreach(Transform slot in currentSpriteSlotHolder)
        {
            Transform spriteTransform = slot.GetChild(0).transform;
            spriteTransform.localScale = new Vector3(spriteTransform.localScale.x * facingDir, spriteTransform.localScale.y, spriteTransform.localScale.z);
        }
    }
    #endregion

    //Draw Delegation
    private void Draw()
    {
        drawer.Update();
    }

    /// <summary>
    /// Returns a boolean of whether the person active
    /// is allowed to interact with this unit.
    /// </summary>
    private bool SelectedByController()
    {
        return Global.ActivePlayerId == Controller.PlayerId;
    }

}
