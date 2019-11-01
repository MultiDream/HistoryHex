using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyEntity : MonoBehaviour
{
    #region Properties
    // Internal variables
    private bool activated = false;
    public Vector3Int Position;
    public float Food;
	public int Manpower;
	public string Name;
	public Player Controller;
	GameObject pathObject;

	// UI_COMponents.
	public GameObject UIComponent; // UIComponentPrefab
    private GameObject UIComponentInstance;

    // SelectionInterface
    private SelectableObj SelectionInterface;
    public EntityDrawer drawer;
    #endregion

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
        bool SelectedByController = Global.ActivePlayerId == Controller.PlayerId;
        if (activated && SelectedByController)
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
        // When active, listen for 7 4 1 and 9 6 3.
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Vector3Int direction = new Vector3Int(0, -1, 1);
            MoveAction(direction);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Vector3Int direction = new Vector3Int(-1, 0, 1);
            MoveAction(direction);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Vector3Int direction = new Vector3Int(-1, 1, 0);
            MoveAction(direction);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            Vector3Int direction = new Vector3Int(1, -1, 0);
            MoveAction(direction);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            Vector3Int direction = new Vector3Int(1, 0, -1);
            MoveAction(direction);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Vector3Int direction = new Vector3Int(0, 1, -1);
            MoveAction(direction);
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
		if (hex == null){
			// do nothing.
		} else {
			//create a path between this tile and that one.
			if (pathObject != null){
				//pathObject.GetComponent<HexPath>().Destroy();
				Destroy(pathObject);
				pathObject = null;
			}
			GameObject armyTile = Global.MapFlyWeight.hexMap[Position];
			
			pathObject = Instantiate(Global.MapFlyWeight.hexPathPrefab);
			HexPath path = pathObject.GetComponent<HexPath>();
			path.Initialize();

			List<GameObject> hexes = Global.MapFlyWeight.adjacencyMap.NearestAstar(armyTile, baseTile);
			path.AddHexes(hexes);
		}
	}

	/// <summary>
	/// Attempts to take control of a tile.
	/// </summary>
	/// <param name="hexTile"></param>
    public void Sieze(GameObject hexTile)
    {
        HexEntity entity = hexTile.GetComponent<HexEntity>();
        entity.Controller = this.Controller;
        Combat(entity.army);
        entity.army = gameObject;
    }

	/// <summary>
    /// Combats another unit.
    /// </summary>
    public void Combat(GameObject otherArmy)
    {
        if (otherArmy != null)
        {
            //Seems to destroy the Army, despite not being passed by refrence.
            Destroy(otherArmy);
        }
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

	/// <summary>
	/// Does everything needed to update the army at the start of the turn.
	/// </summary>
	private void OnStartTurn(){
		// Forage for food. Currently tries to get enough rations for just the current army.
		ForageTile(Manpower);
		Food -= Manpower;

		// Starvation mechanic
		if (Food < 0){
			Manpower += Mathf.FloorToInt(Food);
			Food = 0;
		}

		// Death when no manpower remaining.
		if (Manpower <= 0){
			Destroy(gameObject);
		}

		Debug.Log("Current Food: " + Food);
		Debug.Log("Current Manpower: " + Manpower);
	}

    #region WireSelectionInterface
	/// <summary>
	/// Wires up all the event handlers for the this entity.
	/// </summary>
    private void WireSelectionInterface()
    {
        SelectionInterface.Prepare();
        SelectionInterface.OnSelect += OnSelect;
        SelectionInterface.OnDeselect += OnDeselect;
		SelectionInterface.OnRightClick += OnRightClick;
    }

    private void OnSelect()
    {
        activated = true;
    }

    private void OnDeselect()
    {
        activated = false;
    }

	private void OnRightClick(GameObject other){
		//Depending on the mod, army will do a different action.
		AddSupplyLine(other);
	}
    #endregion

    //Draw Delegation
    private void Draw()
    {
        drawer.Update();
    }
}
