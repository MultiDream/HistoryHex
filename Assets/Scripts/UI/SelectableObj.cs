using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach Selectable Object to introduce UIComponentPrefabs
/// The SelectionController will look for these when a thing is selected.
/// </summary>
public class SelectableObj : MonoBehaviour
{
	public GameObject UIComponentPrefab; //UIComponent
	private GameObject UIComponent;
	// Delegation of the Selection/Unselection methods.
	// Use these to attach the Selection methods programmatically.
	// Are event handles necessary?
	//public delegate void SelectionHandler();
	//public event SelectionHandler OnSelect;
	//public event SelectionHandler OnDeselect;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	// Fired by the SelectionController when the Object is selected.
	public void OnSelected(){
		if (UIComponent != null){
			//Creating a new component without imploding the old one creates memory leaks.
			throw new System.Exception("Imploding Program. UI Must not exist before getting created!");
		}
		UIComponent = Instantiate(UIComponentPrefab);
		//OnSelect(); //Publishes the fact that the object has been selected. Necessary?
	}

	// Fired by the SelectionController when the Object is deselected.
	public void OnDeselected() {
		Destroy(UIComponent);
		UIComponent = null; //For some reason, destroying a component does not actually make it go away?

		//OnDeselect(); //Publishes the fact that the object has been unselected. Necessary?
	}
}
