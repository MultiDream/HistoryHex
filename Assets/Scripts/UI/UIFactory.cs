using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for the construction of UI Components.
/// </summary>
public class UIFactory : MonoBehaviour
{
	public GameObject ArmyUIPrefab;
	public GameObject FlagUIPrefab;
	public GameObject TileUIPrefab;

	public GameObject getUI(){
		GameObject newUI = Instantiate(FlagUIPrefab);
		return newUI;
	}
}
