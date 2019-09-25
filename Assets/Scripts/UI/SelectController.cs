using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Controller responsible for handling mouse input.
/// </summary>
public class SelectController : MonoBehaviour {
	public SelectableObj selectedObj;
	public GameObject ArmyPrefab;

	// Update is called once per frame.
	// Used to determine if something new has been selected.
	void Update() {
		// Left mouse will select a new SelectableObj.
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			if (selectedObj != null) {
				Deselect();
			}

			var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Debug.DrawRay(ray.origin, ray.direction, Color.green, 100f); // only draws once. Re-clicking does nothing
			if (Physics.Raycast(ray, out hit)) {
				var selection = hit.transform;
				selectedObj = selection.GetComponent<SelectableObj>();

				if (selectedObj != null) {
					Select();
				}
			}
		}
		//Right keys will issue commands to selectableObj.
		//Army spawning key.Move code elsewhere at some point.
		else if (Input.GetKeyDown(KeyCode.V)) {
			if (selectedObj != null) {
				Vector3 position = selectedObj.transform.position;
				Quaternion rotation = Quaternion.Euler(0, 0, 0);
				Instantiate(ArmyPrefab, position, rotation);
			}
		}
	}

	private void Select() {
		//Hex Specific Logic. Replace later.
		//selected.drawer.selected = true;
		selectedObj.OnSelected();
	}

	private void Deselect(){
		
		selectedObj.OnDeselected();
		//Hex specific Logic. Replace later.
		//selected.drawer.selected = false;
		//selected = null;
	}
}
