using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Controller responsible for handling mouse input.
/// </summary>
public class SelectController : MonoBehaviour {
	public SelectableObj SelectedObj;
	public GameObject ArmyPrefab;  //Remove this later.

	// Update is called once per frame.
	// Used to determine if something new has been selected.
	void Update() {
		// Left mouse will select a new SelectableObj.
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			if (SelectedObj != null) {
				Deselect();
			}

			var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Debug.DrawRay(ray.origin, ray.direction, Color.green, 100f); // only draws once. Re-clicking does nothing
			if (Physics.Raycast(ray, out hit)) {
				var selectedTransform = hit.transform;
				SelectedObj = selectedTransform.GetComponent<SelectableObj>();

				// If the transform has a selectable Component, run the Selection logic.
				if (SelectedObj != null) {
					Select();
				}
			}
		}
		//Right keys will issue commands to selectableObj.
		//Army spawning key.Move code elsewhere at some point.
		//else if (Input.GetKeyDown(KeyCode.V)) {
		//	if (SelectedObj != null) {
		//		Vector3 position = SelectedObj.transform.position;
		//		Quaternion rotation = Quaternion.Euler(0, 0, 0);
		//		Instantiate(ArmyPrefab, position, rotation);
		//	}
		//}
	}

	private void Select() {
		SelectedObj.OnSelected();
	}

	private void Deselect(){
		SelectedObj.OnDeselected();
	}
}
