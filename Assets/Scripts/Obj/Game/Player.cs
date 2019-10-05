using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The home for player data, to be used by other controllers.
/// Pretty much just an entity. Consider renaming it.
/// </summary>
public class Player : MonoBehaviour
{
	public Color Colour = new Color(); //This is terrible naming. Won't somebody do something?

	public int PlayerId; //unique identifier. Please don't double up on ids. Please.

	public void Start() {
	}

	public void Update() {
		
	}
}
