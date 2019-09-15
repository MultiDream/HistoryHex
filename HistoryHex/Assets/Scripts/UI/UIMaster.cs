using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script's purpose is to manage a series of other UI components.
///	For now, that list includes:
///		1. The Selection Controller
///		2. The Active Player Display.
///		3. The Game Timer.
///		
/// This class isn't ready yet, but eventually all components will be moved here.
/// </summary>
public class UIMaster : MonoBehaviour
{
	public ILedger activeLedger;
	public Player ActivePlayer {
		set {
			Debug.Log(value.Colour);
			transform.GetChild(3).GetComponent<Image>().color = value.Colour;
		} 
	}

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Make sure the correct active player is ready.
    }
}
