using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is an alternative way to organize a hexTile. It will require some refactoring
 * to be useful, but I think its probably best to use singletons for Unity, with some internal hidden data.
 * 
 * I'll go to the Unity meet up and see what people think.
 * */
public class HexSingleton : MonoBehaviour
{
	public float Food;
	public string Name { get; set; }
	public Player Controller { get; set; }


	internal HexDrawer drawer;
    // Start is called before the first frame update
    void Start()
    {
		Initialize();
    }

    // Update is called once per frame.
    void Update()
    {
		// Shows the Food Map.
		if (Input.GetKeyDown(KeyCode.F)){
			drawer.hexCol = new Color(Food/4f,0,0);
		}
		//Shows the Control Map.
		else if (Input.GetKeyDown(KeyCode.G)) {
			if (Controller != null){
				drawer.hexCol = Controller.Colour;
			} else {
				drawer.hexCol = Color.black;
			}
		}
		// Clears the map.
		else if (Input.GetKeyDown(KeyCode.R)) {
			drawer.hexCol = Color.white;
		}
		Draw();
    }

	//Start Delegation
	private void Initialize(){
		Name = "NoMansLand";
		Food = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);
		drawer = new HexDrawer(transform.GetChild(0));
	}

	//Draw Delegation
	private void Draw() {
		drawer.Update();	
	}
}