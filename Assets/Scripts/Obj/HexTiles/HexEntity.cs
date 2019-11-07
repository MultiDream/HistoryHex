using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Does not follow singleton pattern. Refactor to HexEntity.
public class HexEntity : MonoBehaviour
{
    #region Properties
    // Internal variables
    private bool activated = false;
    // This variable keeps track of how many turns have passed
    private int turnCounter = 0;
    // This variable calculates if an attack was made on this tile after your previous turn
    private bool attacked = false;

    //Prefabs
    public GameObject ArmyPrefab;
    //Public variables
    public Vector3Int Position; //Position on the hex grid.
    public float FoodBase;
	public float Food { get; set; }
    public string Name { get; set; }
    public Player Controller { get; set; }
    public EntityDrawer drawer;
    public GameObject army; // make into an array later, when multiple armies can sit on a tile.
    public float TotalPopulation;
    public float FoodPopulation;
    public float SupplyLinePopulation;
    // This dictionary keeps track of labor pools, the key of this dict is a string representing the type of labor and the int
    // represents the amount of a population that is put into this type of labor
    private Dictionary<string,float> laborPoolDict = new Dictionary<string,float>();


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
            drawer.Color = new Color(FoodBase / 4f, 0, 0);
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

	#region Event Infrastructure
	private void WireSelectionInterface()
    {
        SelectionInterface.Prepare();
        SelectionInterface.OnSelect += OnSelect;
        SelectionInterface.OnDeselect += OnDeselect;
		SelectionInterface.OnInitializeUI += OnInitializeUI;
    }

    private void OnSelect()
    {
        activated = true;
    }

    private void OnDeselect()
    {
        activated = false;
    }

	private void OnInitializeUI(UICom com) {
		((UIHex)com).SetText(Name, Controller.PlayerId.ToString(), Food.ToString(), "XXX", Population.ToString(), "XXX", "XXX", "XXX");
	}

	#endregion

	//Start Delegation
	private void Initialize()
    {
        Name = "NoMansLand";
        FoodBase = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);
        drawer = new EntityDrawer(transform.GetChild(0));
        FoodPopulation = 0.0f;
        SupplyLinePopulation = 0.0f;
        TotalPopulation = 0.0f;
        InitializePopulation();
        InitializeLaborPools();
    }

	// Runs during initialization of tiles and sets a base population for each tile
	// The base population now is equal to base food but this can be changed as more is implemented 
	private void InitializePopulation()
    {
        // No Base Food means that no population can be created
        if (FoodBase >= 0)
        {
            TotalPopulation = FoodBase * 100;
        }
    }

    // Runs during initialization of tiles and puts a population into a labor pool. Since food is
    // the default labor pool the entire population is put into the food labor pool. This also creates the types of labor
    // pools by adding them to the laborPoolDict
    private void InitializeLaborPools() {
        // This happens because all the default labor pool poplulation goes into is food
        FoodPopulation = TotalPopulation;
        laborPoolDict.Add("Food", FoodPopulation);
        laborPoolDict.Add("SupplyLines", SupplyLinePopulation);
    }

	#region NextTurn Updates
	// This updates every turn and decides if updatePopulation() is run
	private void checkUpdatePopulation()
    {
        if (turnCounter >= 4 && Food >= 0)
        {
            updatePopulation();
            turnCounter = 0;
        }
    }

    // This updates the Population variables 
    private void updatePopulation()
    {
        TotalPopulation++;
        // Since you add to the total population you must add to a labor pool and food is the default labor pool
        FoodPopulation++;
    }

    // This runs every turn and updates food based on population if not attacked after previous turn
    // The amount of Food added every turn simply equals the population but this can be changed as more is implemented
    private void updateFood()
    {
        if (FoodBase >= 0 & attacked == false)
        {
            Food += TotalPopulation;
        }
    }

    // This runs after every turn run by the Controlling Player
    public void updateTurn()
    {
        updateFood();
        checkUpdatePopulation();
        turnCounter++;
    }

	#endregion

    // NEED TO ADD FUNCTIONS THAT CHECK IF A LABOR POOL SWITCH CAN BE MADE BUT CAN'T IMPLEMENT NOW BECAUSE SUPPLY LINES NOT READY

    // This function actually changes the values in a labor pool after checking if the values in labor pools can be switched
    // Parameters:
    // 1) laborPoolAdd: the labor pool gaining population
    // 2) laborPoolSubtract: the labor pool losing population
    // 3) amount: the amount of population being switched
    public void switchLaborPoolAssignments(string laborPoolAdd, string laborPoolSubtract, float amount) {
        laborPoolDict[laborPoolAdd] += amount;
        laborPoolDict[laborPoolSubtract] -= amount;
    }

	// Returns as much food as possible, given a request.
	public int FoodRequest(int request)
	{
		if (request <= 0){
			return 0;
		}
		if (request > Food)
		{
			request = Mathf.FloorToInt(Food);
		}
		Food -= request;
		return request;
	}
	//Draw Delegation
	private void Draw()
    {
		// Disabled until the drawer is reworked
        //drawer.Update();
    }

	#region Distance Utilities
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
	#endregion
}
