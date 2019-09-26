using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Does not follow singleton pattern. Refactor to HexEntity.
public class HexSingleton : MonoBehaviour
{
	public float Food;
	public string Name { get; set; }
	public Player Controller { get; set; }
	public EntityDrawer drawer;
    // Start is called before the first frame update
    void Start()
    {
		Initialize();
    }

    // Update is called once per frame.
    void Update()
    {
		// Move the responsibility of setting Map Viewing modes to another class later.

		// Shows the Food Map.
		if (Input.GetKeyDown(KeyCode.F)){
			drawer.Color = new Color(Food/4f,0,0);
		}
		//Shows the Control Map.
		else if (Input.GetKeyDown(KeyCode.G)) {
			if (Controller != null){
				drawer.Color = Controller.Colour;
			} else {
				drawer.Color = Color.black;
			}
		}
		// Clears the map.
		else if (Input.GetKeyDown(KeyCode.R)) {
			drawer.Color = Color.white;
		}
		Draw();
    }

	//Start Delegation
	private void Initialize(){
		Name = "NoMansLand";
		Food = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);
		drawer = new EntityDrawer(transform.GetChild(0));
	}

	//Draw Delegation
	private void Draw() {
		drawer.Update();	
	}
}