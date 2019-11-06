using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// Controller responsible for handling mouse input.
/// </summary>
public class SelectController : MonoBehaviour
{
    public SelectableObj SelectedObj;
    public SelectableObj LastSelected;
    public GameObject ArmyPrefab;  //Remove this later.

    // Update is called once per frame.
    // Used to determine if something new has been selected.
    void Update()
    {
        
        //UI components need to block all of this
        
        // Left mouse will select a new SelectableObj.
        KeyDownSelect(KeyCode.Mouse0);
        //KeyDownSelect(KeyCode.Mouse1);
    }

    public void KeyDownSelect(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {

            if (SelectedObj != null)
            {
                Deselect();
                LastSelected = SelectedObj;
            }


            var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction, Color.green, 100f); // only draws once. Re-clicking does nothing
            if (Physics.Raycast(ray, out hit))
            {
                var selectedTransform = hit.transform;
                SelectedObj = selectedTransform.GetComponent<SelectableObj>();
                // If the transform has a selectable Component, run the Selection logic.
                if (SelectedObj != null) 
                {
                    SelectedObj.LastSelectKey = key;  // Better way to do this ? ? 
                    Select();
                }
            }
        }
    }

    private void Select()
    {
        SelectedObj.OnSelected();
    }

    private void Deselect()
    {
        SelectedObj.OnDeselected();
    }

}
