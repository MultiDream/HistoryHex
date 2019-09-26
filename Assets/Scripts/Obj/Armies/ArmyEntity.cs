using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyEntity : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
		// Move the responsibility of setting Unit Viewing modes to another class later.
		// Shows the Controller Color.
		if (Input.GetKeyDown(KeyCode.G)) {
			if (Controller != null) {
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

	void Initialize(){
		Name = "UnnamedArmy";
		Food = Mathf.Floor(Random.value * Global.MAXIMUM_FOOD);
		drawer = new EntityDrawer(transform);
	}

	//Draw Delegation
	private void Draw() {
		drawer.Update();
	}
}
