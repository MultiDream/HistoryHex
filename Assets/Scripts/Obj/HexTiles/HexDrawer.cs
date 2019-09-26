using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Legacy Drawer for HexTiles.
/// </summary>
public class HexDrawer {
	Transform hexMesh;
	public bool selected;
	public Color highlightColor = Color.yellow; //Need to refactor
	public Color hexCol { get; set; }

	//DrawCycle is a timer that will cause the HexDrawer to redraw hexes each time that number of milliseconds passes.
	float drawCycle;
	float drawTimer;

	public HexDrawer(Transform hex) {
		hexMesh = hex;
		hexCol = Color.white;
		drawCycle = 0.50f;
		drawTimer = drawCycle;
	}

	public void Update() {
		if (drawTimer > 0) {
			drawTimer -= Time.deltaTime;
		} else {
			onDrawTimer();
		}

		if (selected && (drawTimer > drawCycle/2)){
			colorHex(highlightColor);
		} else {
			colorHex(hexCol);
		}
	}

	// Colors a tile to the Color it has been set to have.
	public void colorMatBasic() {
		hexMesh.GetComponent<MeshRenderer>().material.color = hexCol;
	}

	public void colorHex(Color col){
		hexMesh.GetComponent<MeshRenderer>().material.color = col;
	}

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
}
