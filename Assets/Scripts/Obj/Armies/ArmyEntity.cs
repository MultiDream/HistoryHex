using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyEntity : MonoBehaviour
{
	// Internal variables
	private bool activated = false;

	public int[] Position; //Position on the hex grid.
	public float Food;
	public string Name { get; set; }
	public Player Controller { get; set; }

	// UI_COMponents.
	public GameObject UIComponent; // UIComponentPrefab
	private GameObject UIComponentInstance;

	// SelectionInterface
	private SelectableObj SelectionInterface;
	public EntityDrawer drawer;
	// Start is called before the first frame update
	void Start()
    {
		Initialize();
	}

    // Update is called once per frame
    void Update()
    {
		// Move the responsibility of setting Unit Viewing modes to another class later.
		// Shows the Controller Color.
		if (Input.GetKeyDown(KeyCode.G)) {
			if (Controller != null) {
				drawer.Color = Controller.Colour;
			} else {
				drawer.Color = Color.black;
			}
		}
		// Clears the map.
		else if (Input.GetKeyDown(KeyCode.R)) {
			drawer.Color = Color.white;
		}

		//If Activated, run the extended activation methods.
		if (activated){
			ActiveUpdate();
		}

		//Draw the Entity.
		Draw();
	}

	void Initialize(){
		Name = "UnnamedArmy";
		Food = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);

		// Create a drawer.
		drawer = new EntityDrawer(transform);

		//Attempt to wire the SelectionInterface.
		SelectionInterface = transform.GetComponent<SelectableObj>();
		if (SelectionInterface == null){
			throw new UnityException("Failed to link Army Entity to a SelectionInterface.");
		} else {
			WireSelectionInterface();
		}

		//Present UI Components.
	}

	private void ActiveUpdate() {
		Debug.Log($"Army now: {activated}!");
		
		//When active, listen for 7 4 1 and 9 6 3.
		if (Input.GetKeyDown(KeyCode.Keypad3)){
			Vector3 moveTo = Global.GetCubicVector(0, -1, 1);
			transform.Translate(moveTo);
		} else if (Input.GetKeyDown(KeyCode.Keypad1)){
			Vector3 moveTo = Global.GetCubicVector(-1, 0, 1);
			transform.Translate(moveTo);
		} else if (Input.GetKeyDown(KeyCode.Keypad4)){
			Vector3 moveTo = Global.GetCubicVector(-1, 1, 0);
			transform.Translate(moveTo);
		} else if (Input.GetKeyDown(KeyCode.Keypad6)) {
			Vector3 moveTo = Global.GetCubicVector(1, -1, 0);
			transform.Translate(moveTo);
		} else if (Input.GetKeyDown(KeyCode.Keypad9)) {
			Vector3 moveTo = Global.GetCubicVector(1, 0, -1);
			transform.Translate(moveTo);
		} else if (Input.GetKeyDown(KeyCode.Keypad7)) {
			Vector3 moveTo = Global.GetCubicVector(0, 1, -1);
			transform.Translate(moveTo);
		}

		return;
	}

	#region WireSelectionInterface
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

	#endregion

	//Draw Delegation
	private void Draw() {
		drawer.Update();
	}
}
