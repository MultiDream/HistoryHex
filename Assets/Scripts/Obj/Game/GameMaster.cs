using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Master Class for the game. Will handle networking infrastructure,
/// or hotseat mechanics, which ever is applicable.
/// </summary>
public class GameMaster : MonoBehaviour {

	//Prefabs needed for this Component and sub components.
	public GameObject playerPrefab;
	public GameObject UIMasterPrefab;

	public int NumberOfPlayers;
	private GameObject[] Players;
	public Map Board;				//Handles map creation.


	// Start is called before the first frame update
	void Start() {
		Players = new GameObject[NumberOfPlayers];
		for (int i=0; i < NumberOfPlayers; i++){
			Players[i] = Instantiate(playerPrefab);
		}
	}

	// Update is called once per frame
	void Update() {

	}

	// Key Bindings
}
