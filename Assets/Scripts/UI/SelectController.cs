using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectController : MonoBehaviour {
	public HexSingleton selected;
	//public GameObject ArmyPrefab;

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			if (selected != null) {
				Deselect();
			}

			var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Debug.DrawRay(ray.origin, ray.direction, Color.green, 100f); // only draws once. Re-clicking does nothing
			if (Physics.Raycast(ray, out hit)) {
				var selection = hit.transform;
				selected = selection.GetComponent<HexSingleton>();

				if (selected != null) {
					Select();
				}
			}
		} 

		//Army spawning key. Move code elsewhere at some point.
		//else if (Input.GetKeyDown(KeyCode.V)){
		//	if (selected != null) {
		//		Vector3 position = selected.transform.position;
		//		Quaternion rotation = Quaternion.Euler(0, 0, 0);
		//		Instantiate(ArmyPrefab, position, rotation);
		//	}
		//}
	}

	private void Select() {
		selected.drawer.selected = true;
		//transform.GetChild(0).GetComponent<Text>().text = selected.Name;
		//transform.GetChild(1).GetComponent<Text>().text = "Food: " + selected.Food.ToString();
	}

	private void Deselect(){
		selected.drawer.selected = false;
		//transform.GetChild(0).GetComponent<Text>().text = "No Tile Selected";
		//transform.GetChild(1).GetComponent<Text>().text = "-";
		selected = null;
	}
}
