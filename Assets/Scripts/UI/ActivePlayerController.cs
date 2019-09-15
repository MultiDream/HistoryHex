using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Temporary interface for the player that is currently active.
/// </summary>
public class ActivePlayerController : MonoBehaviour
{
	public Player ActivePlayer {
		get {
			return ActivePlayer;
		}
		set{
			ActivePlayer = value;
			SetExtension();
		}
	}

	public void SetExtension(){
		transform.GetChild(3).GetComponent<Image>().color = ActivePlayer.Colour;
	}


	//Don't use.
	public void Start() {
		
	}

	public void Update() {
		
	}
}
