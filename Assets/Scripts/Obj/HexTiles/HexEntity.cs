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
	public int[] Position; //Position on the hex grid.
	public float Food;
	public string Name { get; set; }
	public Player Controller { get; set; }
	public EntityDrawer drawer;
	public GameObject army; // make into an array later, when multiple armies can sit on a tile.
	public float Population;
  
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
		if (SelectionInterface == null) {
			throw new UnityException("Failed to link Army Entity to a SelectionInterface.");
		} else {
			WireSelectionInterface();
		}

		//If Activated, run the extended activation methods.
		if (activated) {
			ActiveUpdate();
		}

		Draw();
    }

	//If Activated, run the extended activation methods.
	private void ActiveUpdate(){
		// Army spawn code.
		if (Input.GetKeyDown(KeyCode.V)){
			Vector3 position = transform.position;
			Quaternion rotation = Quaternion.Euler(0, 0, 0);
			this.army = Instantiate(ArmyPrefab, position, rotation);

			//Set an army up.
			ArmyEntity armyEntity = army.transform.GetComponent<ArmyEntity>();
			armyEntity.Position = Position;
			armyEntity.Controller = Controller;
			
		}
	}

	private void MapDrawingUpdater(){
		// Shows the Food Map.
		if (Global.CurrentMapMode == MapMode.Food) {
			drawer.Color = new Color(Food / 4f, 0, 0);
		}

		//Shows the Control Map.
		else if (Global.CurrentMapMode == MapMode.Controller) {
			if (Controller != null) {
				drawer.Color = Controller.Colour;
			} else {
				drawer.Color = Color.black;
			}
		}
	}

	private void WireSelectionInterface() {
		SelectionInterface.Prepare();
		SelectionInterface.OnSelect += OnSelect;
		SelectionInterface.OnDeselect += OnDeselect;
	}


	private void OnSelect() {
		activated = true;
	}

	private void OnDeselect() {
		activated = false;
	}

	//Start Delegation
	private void Initialize(){
		Name = "NoMansLand";
		Food = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);
		drawer = new EntityDrawer(transform.GetChild(0));
		InitializePopulation();
	}

	// Runs during initialization of tiles and sets a base population for each tile
	// The base population now is equal to base food but this can be changed as more is implemented 
	private void InitializePopulation() {
		// No food means that no population can be created
		if (Food >= 0) {
			Population = Food;
		}
	}

	// This updates every turn and decides if updatePopulation() is run
	private void checkUpdatePopulation() {
		if (turnCounter >= 4 && Food >= 0) {
			updatePopulation();
			turnCounter = 0;
		}
	}

	// This updates the Population variables 
	private void updatePopulation() {
		Population++;
	}

	// This runs every turn and updates food based on population if not attacked after previous turn
	// The amount of Food added every turn simply equals the population but this can be changed as more is implemented
	private void updateFood() {
		if (Food >= 0 & attacked == false) {
			Food += Population;
		}
	}

	// This runs after every turn run by the Controlling Player
	public void updateTurn() {
		updateFood();
		checkUpdatePopulation();
		turnCounter++;
	}

	//Draw Delegation
	private void Draw() {
		drawer.Update();	
	}
}
