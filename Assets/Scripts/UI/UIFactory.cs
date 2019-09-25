using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for the construction of UI Components.
/// </summary>
public class UIFactory : MonoBehaviour
{
	public GameObject ArmyUIPrefab;
	public GameObject TileUIPrefab;

	public GameObject getUI(){
		GameObject newUI = Instantiate(ArmyUIPrefab);
		return newUI;
	}

	public GameObject getWiredUI(ref KeyboardController keyboard) {
		GameObject newUI = Instantiate(ArmyUIPrefab);
		// newUI.GetComponent<> Oh shit. This doesn't support interfaces.
		return newUI;
	}
}
