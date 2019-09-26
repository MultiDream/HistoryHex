using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDrawer
{
	Transform Mesh;
	public bool selected;
	public Color HighlightColor = Color.yellow; //Need to refactor
	public Color Color { get; set; }

	//DrawCycle is a timer that will cause the HexDrawer to redraw hexes each time that number of milliseconds passes.
	float drawCycle;
	float drawTimer;

	public EntityDrawer(Transform transform) {
		Mesh = transform;
		Color = Color.white;
		drawCycle = 0.50f;
		drawTimer = drawCycle;
	}

	public void Update() {
		if (drawTimer > 0) {
			drawTimer -= Time.deltaTime;
		} else {
			onDrawTimer();
		}

		if (selected && (drawTimer > drawCycle / 2)) {
			colorHex(HighlightColor);
		} else {
			colorHex(Color);
		}
	}

	// Colors a tile to the Color it has been set to have.
	public void colorMatBasic() {
		Mesh.GetComponent<MeshRenderer>().material.color = Color;
	}

	/* Colors a tile what ever the caller wants it to be colored.
	 * This is very short term, it goes away after the timer.
	 * Used the Color Property for longer term changes to color.
	 * */
	public void colorHex(Color col) {
		Mesh.GetComponent<MeshRenderer>().material.color = col;
	}

	#region Timed Events
	//The timer used here should probably be a static timer declared elsewhere in the future.
	//Reset the Draw Timer.
	private void resetDrawTimer() {
		drawTimer = drawCycle;
	}

	//Perform the DrawTimer action.
	private void onDrawTimer() {

		// Makes it repeat.
		resetDrawTimer();

		//Action to do on the draw timer.
		colorMatBasic();
	}
	#endregion
}
