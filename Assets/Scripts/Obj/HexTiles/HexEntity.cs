using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Does not follow singleton pattern. Refactor to HexEntity.
public class HexEntity : MonoBehaviour
{
    #region Properties
    // Internal variables
    private bool activated = false;

    //Prefabs
    public GameObject ArmyPrefab;

    //Public variables
    public Vector3Int Position; //Position on the hex grid.
    public float Food;
    public string Name { get; set; }
    public Player Controller { get; set; }
    public EntityDrawer drawer;
    public GameObject army; // make into an array later, whne multiple armies can sit on a tile.
                            // SelectionInterface
    private SelectableObj SelectionInterface;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame.
    void Update()
    {
        MapDrawingUpdater();

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

        //If Activated, run the extended activation methods.
        if (activated)
        {
            ActiveUpdate();
        }

        Draw();
    }

    //If Activated, run the extended activation methods.
    private void ActiveUpdate()
    {
        // Army spawn code.
        if (Input.GetKeyDown(KeyCode.V))
        {
            Vector3 position = transform.position;
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            this.army = Instantiate(ArmyPrefab, position, rotation);

            //Set an army up.
            ArmyEntity armyEntity = army.transform.GetComponent<ArmyEntity>();
            armyEntity.Position = Position;
            armyEntity.Controller = Controller;

        }
    }

    private void MapDrawingUpdater()
    {
        // Shows the Food Map.
        if (Global.CurrentMapMode == MapMode.Food)
        {
            drawer.Color = new Color(Food / 4f, 0, 0);
        }

        //Shows the Control Map.
        else if (Global.CurrentMapMode == MapMode.Controller)
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

    private void WireSelectionInterface()
    {
        SelectionInterface.Prepare();
        SelectionInterface.OnSelect += OnSelect;
        SelectionInterface.OnDeselect += OnDeselect;
    }


    private void OnSelect()
    {
        activated = true;
    }

    private void OnDeselect()
    {
        activated = false;
    }

    //Start Delegation
    private void Initialize()
    {
        Name = "NoMansLand";
        Food = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);
        drawer = new EntityDrawer(transform.GetChild(0));
    }

    //Draw Delegation
    private void Draw()
    {
        drawer.Update();
    }

    //Draw Delegation
    private void Draw()
    {
        drawer.Update();
    }


    // Wraps position vector operations. Returns distance in each coordinate.
    public Vector3Int CoordinateDistance(HexEntity hex)
    {
        Vector3Int distance = (this.Position - hex.Position);
        return new Vector3Int(Mathf.Abs(distance.x), Mathf.Abs(distance.y), Mathf.Abs(distance.z));
    }

    // Return coordinate distance as magnitude. Divide by 2 for cubic
    public float CubicDistance(HexEntity hex)
    {
        return CoordinateDistance(hex).magnitude / 2;
    }

    // Wraps Position distance function
    public float Distance(HexEntity hex)
    {
        return (this.Position - hex.Position).magnitude;
    }

    // Simple logic to check adjacent based on cubic distance logic (two coordinates have Distance of 1)
    public bool Adjacent(HexEntity hex)
    {
        Vector3Int distance = this.CoordinateDistance(hex);
        return ((distance.x == 1 && distance.y == 1 && distance.z == 0) ||
                (distance.x == 1 && distance.y == 0 && distance.z == 1) ||
                (distance.x == 0 && distance.y == 1 && distance.z == 1));
    }
}