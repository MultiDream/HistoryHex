using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Does not follow singleton pattern. Refactor to HexEntity.
public class HexEntity : MonoBehaviour
{
	// Internal variables
	private bool activated = false;

	//Prefabs
	public GameObject ArmyPrefab;

	//Public variables
	public int[] Position; //Position on the hex grid.
	public float Food;
	public string Name { get; set; }
	public Player Controller { get; set; }
	public EntityDrawer drawer;

	// SelectionInterface
	private SelectableObj SelectionInterface;

	// Start is called before the first frame update
	void Start()
    {
		Initialize();
    }

    // Update is called once per frame.
    void Update()
    {
		// Move the responsibility of setting Map Viewing modes to another class later.

		// Shows the Food Map.
		if (Input.GetKeyDown(KeyCode.F)){
			drawer.Color = new Color(Food/4f,0,0);
		}
		//Shows the Control Map.
		else if (Input.GetKeyDown(KeyCode.G)) {
			if (Controller != null){
				drawer.Color = Controller.Colour;
			} else {
				drawer.Color = Color.black;
			}
		}
		// Clears the map.
		else if (Input.GetKeyDown(KeyCode.R)) {
			drawer.Color = Color.white;
		}

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
			GameObject army = Instantiate(ArmyPrefab, position, rotation);

			//Set an army up.
			army.transform.GetComponent<ArmyEntity>().Position = Position;
			army.transform.GetComponent<ArmyEntity>().Controller = Controller;
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
	}

	//Draw Delegation
	private void Draw() {
		drawer.Update();	
	}
}