using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyEntity : MonoBehaviour
{
    #region Properties
    // Internal variables
    private bool activated = false;
	private bool hasMoved = false;
	public Vector3Int Position;
    public float Food;
	public int Manpower;
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

	private List<HexPath> supplyLines;
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
    }

	// Remove the Event Listener. May no longer be required, but not sure.
	private void OnDestroy() {
		Destroy(SelectionInterface);
		Destroy(pathObject);
		Global.GM.NextTurn -= OnStartTurn;

	}

	void Initialize()
    {
		supplyLines = new List<HexPath>();
        Name = "UnnamedArmy";
        Food = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);
		Manpower = 100;
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
		Global.GM.NextTurn += OnStartTurn;

		//Present UI Components.
	}

    private void ActiveUpdate()
    {
		//Check for the Mode Change key.
		if (Input.GetKeyDown(KeyCode.M))
		{
			Debug.Log("In correct Mode!");
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
		Destroy(pathObject);
        Vector3 moveTo = Global.GetCubicVector(direction.x, direction.y, direction.z);
        Vector3Int nextPos = new Vector3Int(Position.x + direction.x, Position.y + direction.y, Position.z + direction.z);
        if (Global.MapFlyWeight.HasHexAtCubic(nextPos))
        {
            //Get the tile for any operations that might be necessary.
            GameObject nextTile = Global.MapFlyWeight.hexMap[nextPos];
            GameObject currentTile = Global.MapFlyWeight.hexMap[Position];

            HexEntity currentHexEntity = currentTile.GetComponent<HexEntity>();
            currentHexEntity.army = null;

            Sieze(nextTile);
            transform.Translate(moveTo);
            Position = nextPos;
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

			List<GameObject> hexes = Global.MapFlyWeight.getPlayerAdjacencyMap(this.Controller).NearestAstar(armyTile, baseTile);
			path.AddHexes(hexes);
			supplyLines.Add(path);
		}
	}

	public void RefreshSupplyLines(){
		foreach (HexPath path in supplyLines){
			List<GameObject> hexes = Global.MapFlyWeight.getPlayerAdjacencyMap(this.Controller).NearestAstar(path.GetHex(0), path.GetHex(path.Length()));
			path.Refresh(hexes);
		}
	}

	/// <summary>
	/// Moves this army to another tile.
	/// </summary>
	/// <param name="targetTile"></param>
	public void Move(GameObject targetTile){
		// Add logic to check what the other is, and attempt to create a path to that location.
		HexEntity TargetHex = targetTile.GetComponent<HexEntity>();

		// Verify the action should be taken.
		// Did you know C# thinks & has higher priority then | ? Rediculous!
		if (TargetHex == null || hasMoved ) 
		{
			// Do nothing.
		} 
		else 
		{
			// checks to see if the tile can be moved to.
			HexEntity myHex = Global.MapFlyWeight.hexMap[Position].GetComponent<HexEntity>();
			if ( TargetHex.Adjacent(myHex) )
			{
				Vector3Int direction = TargetHex.Position - myHex.Position;
				MoveAction(direction);
				hasMoved = true;
			}
		}
	}
	/// <summary>
	/// Attempts to take control of a tile.
	/// </summary>
	/// <param name="hexTile"></param>
    public void Sieze(GameObject hexTile)
    {
        HexEntity entity = hexTile.GetComponent<HexEntity>();
        bool wonCombat = Combat(entity.army);
		if (wonCombat){
			Global.MapFlyWeight.TransferHexOwner(hexTile, this.Controller);
			entity.army = gameObject;
		}
    }

	/// <summary>
    /// Combats another unit. Return true if winning combat, false otherwise
    /// </summary>
    public bool Combat(GameObject otherArmy)
    {
        if (otherArmy != null)
        {
            //Seems to destroy the Army, despite not being passed by refrence.
            Destroy(otherArmy);
        }
		return true;
    }

	/// <summary>
	/// Attempt to pull food from the tile.
	/// Further testing required.
	/// </summary>
	private void ForageTile(int amount){

		int collected = Global.MapFlyWeight.hexMap[Position].GetComponent<HexEntity>().FoodRequest(amount); //Damn this is long.
		if (collected < amount){
			if (pathObject != null){
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
	private void OnStartTurn() {
		//Set has moved to false.
		hasMoved = false;

		// Forage for food. Currently tries to get enough rations for just the current army.
		ForageTile(Manpower);
		Food -= Manpower;

		// Starvation mechanic
		if (Food < 0) {
			Manpower += Mathf.FloorToInt(Food);
			Food = 0;
		}

		// Death when no manpower remaining.
		if (Manpower <= 0) {
			Destroy(gameObject);
		}

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
        //pass function to be executed on 
    }

    private void OnDeselect()
    {
        activated = false;
    }

	/// <summary>
	/// What to do when something has been right clicked.
	/// </summary>
	/// <param name="other">The object that has been picked with a raycast.</param>
	private void OnRightClick(GameObject other){
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

    private void OnInitializeUI(UICom com) {
        UIArmy uiArmy = (UIArmy)com;
        uiArmy.SetText(Name, Controller.PlayerId.ToString(), Food.ToString(), "", Manpower.ToString(), "","","");
        void ArmyMove() {ActionMode = ArmyActionMode.Move;}
        void ArmySupply() {ActionMode = ArmyActionMode.SetSupply;}
        uiArmy.SetButtonListeners(ArmyMove, ArmySupply);
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
	private bool SelectedByController(){
		return Global.ActivePlayerId == Controller.PlayerId;
	}
}
