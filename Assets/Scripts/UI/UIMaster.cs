using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script's purpose is to manage a series of other UI components.
/// It should always be attached to a UIPrefab.
/// </summary>
public class UIMaster : MonoBehaviour
{
	/* We still need to decide the exact process of registering and unregistering UIComponents. */

	private Dictionary<string,GameObject> UIComponents;
	public KeyboardController keyboard;

    // Start is called before the first frame update
    void Start(){
		keyboard.BindKey(KeyCode.K, K_Key);
    }

    // Update is called once per frame
    void Update(){
    }

	//register a UIComponent
	public void RegisterUIComponent(){
	}

	//unregister a UIComponent
	public void UnregisterUIComponent(){
	}

	// Key bindings
	void K_Key(){
		Debug.Log("K_Key_Pressed");
	}
}
