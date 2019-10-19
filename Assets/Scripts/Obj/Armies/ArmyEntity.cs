using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyEntity : MonoBehaviour
{
    #region Properties
    // Internal variables
    private bool activated = false;

    public Vector3Int Position; //Position on the hex grid.
    public float Food;
    public string Name { get; set; }
    public Player Controller { get; set; }
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

    void Initialize()
    {
        Name = "UnnamedArmy";
        Food = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);

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
    #endregion

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
		//Add logic to check what the other is, and attempt to create a path to that location.
		HexEntity hex = other.GetComponent<HexEntity>();
		if (hex == null){
			// do nothing.
		} else {
			//create a path between this tile and that one.
			if (pathObject != null){
				Destroy(pathObject);
				pathObject = null;
			}
			GameObject baseTile = Global.MapFlyWeight.hexMap[Position];
			GameObject target = other;
			
			pathObject = Instantiate(Global.MapFlyWeight.hexPathPrefab);
			HexPath path = pathObject.GetComponent<HexPath>();
			path.Initialize();
			List<GameObject> hexes = Global.MapFlyWeight.adjacencyMap.NearestAstar(baseTile, target);
			path.AddHexes(hexes);
		}
	}
    #endregion

    //Draw Delegation
    private void Draw()
    {
        drawer.Update();
    }
}
