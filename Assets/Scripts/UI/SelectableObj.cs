using System.Collections;
using UnityEngine;

/// <summary>
/// Attach SelectableObj to introduce an interface for the Selection Controller.
/// The SelectionController will look for these when raycasting clicks.
/// Think of an advanced Factory Pattern, but with more responsibilites.
/// </summary>
public class SelectableObj : MonoBehaviour
{
	public GameObject UIComponentPrefab; //UIComponent. Attach manually.
	private GameObject UIComponent;
	private bool active = false;
	// This is just to inform the entity script what's going on,
	// we use this to hide away the identity of the
	// the entity script. Im sure there is a way to
	// use interfaces, but I don't know that way.

	public delegate void SelectionHandler();
	public event SelectionHandler OnSelect;
	public event SelectionHandler OnDeselect;

	public delegate void RightClickHandler(GameObject other);
	public event RightClickHandler OnRightClick;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {  
		if (active){
			ActiveUpdate();
		}
    }

	// Logic that only runs if this object is active.
	private void ActiveUpdate(){
		if (Input.GetKeyDown(KeyCode.Mouse1)) { //Right Click.
			var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Debug.DrawRay(ray.origin, ray.direction, Color.green, 100f); // only draws once. Re-clicking does nothing
			if (Physics.Raycast(ray, out hit)) {
				var selectedTransform = hit.transform;
				GameObject SelectedObj = selectedTransform.gameObject;

				// throw the right clicked object into the right click event handler.
				OnRightClick(SelectedObj);
			}
		}
	}
	// Fired by the SelectionController when the Object is selected.
	// Basically a factory.
	public void OnSelected(){
		active = true;
		// Create the UI component if a prefab has been defined.
		if (UIComponentPrefab){
			if (UIComponent != null) {
				//Creating a new component without imploding the old one creates memory leaks.
				throw new System.Exception("Memory Leak vulnerability detected. UI Must not exist before getting created!");
			}
			UIComponent = Instantiate(UIComponentPrefab);
		}

		// Do not simplify delegation, intellisense is wrong.
		if (OnSelect != null){
			OnSelect(); //Publishes the fact that the object has been selected.
		} else {
			Debug.Log("No listeners for SelectableObj.OnSelect");
		}
	}

	// Fired by the SelectionController when the Object is deselected.
	// Cleans up after itself.
	public void OnDeselected() {
		active = false;
		// Destroy the UI Component if it exists.
		if (UIComponent != null){
			Destroy(UIComponent);
			UIComponent = null; //This isn't handled by Destroy(), and can create wacky results if not done.
		}

		// Do not simplify delegation, intellisense is wrong.
		if (OnDeselect != null){
			OnDeselect(); //Publishes the fact that the object has been unselected.
		} else {
			Debug.Log("No listeners for SelectableObj.OnDeselect");
		}
	}

	/// <summary>
	/// Event that handles right clicking if another object has been selected.
	/// </summary>
	/// <param name="other">The object that has been right clicked on.</param>
	public void OnRightClicked(GameObject other){
		OnRightClick(other);
		return;
	}

	// Prepares events to be subscribed to.
	// When adding new events, prepare them so that the selection object does not implode when an event fires.
	public void Prepare(){
		OnSelect = new SelectionHandler(() => PrepareDelegate("OnSelect"));
		OnDeselect = new SelectionHandler(() => PrepareDelegate("OnDeselect"));
		OnRightClick = new RightClickHandler((other) => PrepareDelegate("OnRightClick"));
	}

	// Default Delegate to use when preparing subscribers.
	private void PrepareDelegate(string eventName){
		Debug.Log($"Publishing event: {eventName}.");
	}
}
