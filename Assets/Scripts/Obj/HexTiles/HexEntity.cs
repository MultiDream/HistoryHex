using System.Collections;
using System.Linq;
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

    public Renderer hexbase;

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
    public HexPopulation hexPopulation;

	private float _totalPopulation;
	public float TotalPopulation {
		get
		{
			return _totalPopulation;
		} 
		set 
		{
			allocateLabor();
			_totalPopulation = value;
		}
	}

    // This dictionary keeps track of labor pools, the key of this dict is a string representing the type of labor and the int
    // represents the amount of a population that is put into this type of labor
    private Dictionary<LaborPool,float> laborPool = new Dictionary<LaborPool, float>();

	//Supply line related variables
	private Dictionary<LaborPool, float> spentLaborPool = new Dictionary<LaborPool, float>();
	public List<TransportOrder> supplyLines = new List<TransportOrder>();

    // SelectionInterface
    private SelectableObj SelectionInterface;

	//KeyBindings
	KeyCode raiseArmy = KeyCode.V;

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
        if (Input.GetKeyDown(raiseArmy))
        {
			RaiseArmy();
        }
    }

	/// <summary>
	/// Determines whether the controller is the one who has selected this object.
	/// </summary>
	/// <returns></returns>
	private bool SelectedByController(){
		return Controller.PlayerId == Global.ActivePlayerId;
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
        ChangeMaterial(Controller.Colour);
    }

    private void OnDeselect()
    {
        activated = false;
        ChangeMaterial(new Color(0,0,0,0));
    }

	private void OnInitializeUI(UICom com) {
		UIHex uiHex = (UIHex)com;
		float expectedNextFood = laborPool[LaborPool.Food] * FoodBase;

		uiHex.SetText(Name, Controller.PlayerId.ToString(), Food.ToString(),
		(expectedNextFood).ToString(), TotalPopulation.ToString(),
		Mathf.FloorToInt(TotalPopulation * 0.02f).ToString(),
		(foodNeed()).ToString(), laborPool[LaborPool.Supply].ToString() + " / " + supplyNeed());

		
		if (allowArmySpawn())
		{
			uiHex.SetButtonListeners(RaiseArmy);
			uiHex.AllowRaiseArmy();
		}
		else 
		{
			uiHex.DenyRaiseArmy();
		}
	}

	private float foodNeed(){
		float need = 0;
		foreach(TransportOrder order in supplyLines ){
			need += order.amount;
		}
		return need;
	}
	#endregion

	//Start Delegation
	private void Initialize()
    {
		// Linked to the GM's next turn in the Map class.
        Name = "Village";
        FoodBase = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);
        drawer = new EntityDrawer(transform.GetChild(0));
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
		else 
		{
			TotalPopulation = 0.0f;
		}
        hexPopulation.SetPopulation(TotalPopulation);
    }

	private bool allowArmySpawn() {
		return SelectedByController() && TotalPopulation >= 200 && army == null;
	}

	public void RaiseArmy(){
		// Army spawn code.
		if (allowArmySpawn()) {
			TotalPopulation -= 100;
			Vector3 position = transform.position;
			Quaternion rotation = Quaternion.Euler(0, 0, 0);
			this.army = Instantiate(ArmyPrefab, position, rotation);

			//Set an army up.
			ArmyEntity armyEntity = army.transform.GetComponent<ArmyEntity>();
			armyEntity.Position = Position;
			armyEntity.Controller = Controller;
		}
	}

	#region NextTurn Updates
	// This updates every turn and decides if updatePopulation() is run
	private void checkUpdatePopulation()
    {
		updatePopulation();
		if (turnCounter >= 4 && Food >= 0)
        {
            turnCounter = 0;
        }
    }

    // This updates the Population variables 
    private void updatePopulation()
    {
		int increase = Mathf.FloorToInt(TotalPopulation * 0.02f);
		TotalPopulation += increase;

        hexPopulation.SetPopulation(TotalPopulation);
    }

    // This runs every turn and updates food based on population if not attacked after previous turn
    // The amount of Food added every turn simply equals the population but this can be changed as more is implemented
    private void updateFood()
    {
        if (FoodBase >= 0 & attacked == false)
        {
            Food += laborPool[LaborPool.Food] * FoodBase; //Times some constant.
        }
    }

    // This runs after every turn run by the Controlling Player
    public void updateTurn()
    {
		ResetSpentLabor();
        updateFood();
        checkUpdatePopulation();
		allocateLabor();
        turnCounter++;
    }

    public void UpdateController(Player newController) {
        Controller = newController;
        Material m = hexbase.material;
        m.SetColor("_Color", Controller.Colour);
    }

	#endregion

	#region LaborPools
	// NEED TO ADD FUNCTIONS THAT CHECK IF A LABOR POOL SWITCH CAN BE MADE BUT CAN'T IMPLEMENT NOW BECAUSE SUPPLY LINES NOT READY

	// This function actually changes the values in a labor pool after checking if the values in labor pools can be switched
	// Parameters:
	// 1) laborPoolAdd: the labor pool gaining population
	// 2) laborPoolSubtract: the labor pool losing population
	// 3) amount: the amount of population being switched
	public void switchLaborPoolAssignments(LaborPool laborPoolAdd, LaborPool laborPoolSubtract, float amount) {
		if(laborPool[laborPoolSubtract] >= amount)
		{
			laborPool[laborPoolAdd] += amount;
			laborPool[laborPoolSubtract] -= amount;
		}
        else
		{
			laborPool[laborPoolAdd] += laborPool[laborPoolSubtract];
			laborPool[laborPoolSubtract] = 0;
		}
    }

	// Runs during initialization of tiles and puts a population into a labor pool. Since food is
	// the default labor pool the entire population is put into the food labor pool. This also creates the types of labor
	// pools by adding them to the laborPoolDict
	private void InitializeLaborPools() {
		// This happens because all the default labor pool poplulation goes into is food

		// Somehow, these are already being added BEFORE we define them here.
		// Not sure how or why. 
		//laborPool.Add(LaborPool.Food, TotalPopulation);
		//laborPool.Add(LaborPool.Supply, 0);
		//spentLaborPool.Add(LaborPool.Food, 0);
		//spentLaborPool.Add(LaborPool.Supply, 0);
		laborPool[LaborPool.Food] =  TotalPopulation;
		laborPool[LaborPool.Supply] = 0;
		spentLaborPool[LaborPool.Food] = 0;
		spentLaborPool[LaborPool.Supply] = 0;
	}

	//Sets the spent labor to zero.
	private void ResetSpentLabor(){
		List<LaborPool> pools = new List<LaborPool>(spentLaborPool.Keys);
		foreach (LaborPool pool in pools){
			spentLaborPool[pool] = 0;
		}
	}

	//Determines the allocation of supply line labor.
	private void allocateLabor(){
		int supplyDemand = supplyNeed();
		laborPool[LaborPool.Food] = Mathf.Max(TotalPopulation-supplyDemand,0) ;
		laborPool[LaborPool.Supply] = Mathf.Min(supplyDemand,TotalPopulation);
	}

	private int supplyNeed(){
		int supplyDemand = 0;
		foreach (TransportOrder order in supplyLines) {
			supplyDemand += order.amount * (order.requestor.Length() - 1);
		}
		return supplyDemand;
	}
	/// <summary>
	/// Returns as much food as possible, given a request.
	/// </summary>
	/// <param name="request">The amount of food beind requested</param>
	/// <param name="distance">The distance the food will have to travel.</param>
	/// <returns></returns>
	public int FoodRequest(int request, int distance) 
	{
		// Do not allow negative requests.
		if (request <= 0) {
			return 0;
		}

		int payload = 0;

		// If the food requested exceeds the amount in the tile,
		// you will only recieve what the tile can provide.
		if (request > Food) {
			payload = Mathf.FloorToInt(Food);
		} else {
			payload = request;
		}

		//Calculate Transport Cost.
		int laborCost = payload * distance;

		//Resize the payload to meet available resources for the request.
		int availableTransport = Mathf.FloorToInt(laborPool[LaborPool.Supply] - spentLaborPool[LaborPool.Supply]);
		if (laborCost > availableTransport)
		{
			// Reduce the payload to what can actually be transported.
			payload = availableTransport / distance;
			spentLaborPool[LaborPool.Supply] += availableTransport;
		}
		else 
		{
			spentLaborPool[LaborPool.Supply] += laborCost;
		}
		Food -= payload;
		return payload;
	}

	public void CreateOrder(HexPath path)
	{
		TransportOrder order = new TransportOrder();
		order.requestor = path;
		order.amount = path.army.Manpower;
		supplyLines.Add(order);
		allocateLabor();
	}

	public void DeleteOrder(HexPath path) {
		TransportOrder order = supplyLines.First(x => x.requestor == path);
		supplyLines.Remove(order);
		allocateLabor();
	}

	#endregion


	//Draw Delegation
	private void Draw()
    {
		// Disabled until the drawer is reworked
        //drawer.Update();
    }

    void ChangeMaterial(Color c) {
        Material m = hexbase.material;
        m.SetColor("_Emission", c);
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
