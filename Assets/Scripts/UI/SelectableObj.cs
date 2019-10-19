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
    // This is just to inform the entity script what's going on,
    // we use this to hide away the identity of the
    // the entity script. Im sure there is a way to
    // use interfaces, but I don't know that way.

    public delegate void SelectionHandler();
    public event SelectionHandler OnSelect;
    public event SelectionHandler OnDeselect;
    public KeyCode LastSelectKey;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    // Fired by the SelectionController when the Object is selected.
    // Basically a factory.
    public void OnSelected()
    {

        // Create the UI component if a prefab has been defined.
        if (UIComponentPrefab)
        {
            if (UIComponent != null)
            {
                //Creating a new component without imploding the old one creates memory leaks.
                throw new System.Exception("Memory Leak vulnerability detected. UI Must not exist before getting created!");
            }
            UIComponent = Instantiate(UIComponentPrefab);
        }

        // Do not simplify delegation, intellisense is wrong.
        if (OnSelect != null)
        {
            OnSelect(); //Publishes the fact that the object has been selected.
        }
        else
        {
            Debug.Log("No listeners for SelectableObj.OnSelect");
        }
    }

    // Fired by the SelectionController when the Object is deselected.
    public void OnDeselected()
    {

        // Destroy the UI Component if it exists.
        if (UIComponent != null)
        {
            Destroy(UIComponent);
            UIComponent = null; //This isn't handled by Destroy(), and can create wacky results if not done.
        }

        // Do not simplify delegation, intellisense is wrong.
        if (OnDeselect != null)
        {
            OnDeselect(); //Publishes the fact that the object has been unselected.
        }
        else
        {
            Debug.Log("No listeners for SelectableObj.OnDeselect");
        }
    }

    // Prepares events to be subscribed to.
    public void Prepare()
    {
        OnSelect = new SelectionHandler(() => PrepareDelegate("OnSelect"));
        OnDeselect = new SelectionHandler(() => PrepareDelegate("OnDeselect"));
    }

    // Default Delegate to use when preparing subscribers.
    private void PrepareDelegate(string eventName)
    {
        Debug.Log($"Publishing event: {eventName}.");
    }
}
