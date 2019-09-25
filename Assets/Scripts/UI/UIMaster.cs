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

	public GameObject subComponent;
	public KeyboardController keyboard;			// Needs to be passed in.
	public SelectController selectController;	// Same deal.

    // Start is called before the first frame update
    void Start(){
		BindKeys();
		keyboard.Listening = true;
    }

    // Update is called once per frame
    void Update(){
    }

	//register a UIComponent
	public void RegisterUIComponent(){
		if (subComponent == null){
			UIFactory factory = transform.GetComponent<UIFactory>();
			subComponent = factory.getUI();
			//subComponent.GetComponent<ISelectable>(); Shit, I got a bad feeling about this.
		}
	}

	//unregister a UIComponent
	public void UnregisterUIComponent(){
		Destroy(subComponent);
		subComponent = null;
	}

	// Key bindings
	void BindKeys(){
		keyboard.BindKey(KeyCode.J, J_Key);
		keyboard.BindKey(KeyCode.K, K_Key);
	}

	void J_Key() {
		Debug.Log("Registering UI");
		RegisterUIComponent();
	}
	
	void K_Key() {
		Debug.Log("Unregistering UI");
		UnregisterUIComponent();
	}
}
