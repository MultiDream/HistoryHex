using System.Collections;
using System.Collections.Generic;
using System;

//using UnityEngine.Events; //Bah, why use this?
using UnityEngine;



// Relevant Delegates:
public delegate void NextTurnHandler(); //Handles moving game forward one turn.

/// <summary>
/// Master Class for the game. Will handle networking infrastructure,
/// or hotseat mechanics, which ever is applicable.
/// </summary>
public class GameMaster : MonoBehaviour {

	//Prefabs needed for this Component and sub components.
	public GameObject playerPrefab;
	public GameObject UIMasterPrefab;

	public int NumberOfPlayers;
	public int currentPlayer = 0;
	public GameObject[] Players;
	public Map Board;				//Handles map creation.

	// Start is called before the first frame update
	void Start() {
		Players = new GameObject[NumberOfPlayers];
		for (int i = 0; i < NumberOfPlayers; i++){
			Players[i] = Instantiate(playerPrefab);
			Players[i].transform.GetComponent<Player>().Colour = UnityEngine.Random.ColorHSV();
		}

		Board.InitMap();
		Player[] _players = new Player[NumberOfPlayers];
		for(int i = 0; i < NumberOfPlayers; i++){
			_players[i] = Players[i].GetComponent<Player>();
		}
		Board.setControl(_players); //Needs to run after the map is generated.
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)){
			Space_Key();
		}
	}

	#region KeyBindings
	/*-------------------------------------------------
	 *                  Key Bindings
	 *-----------------------------------------------*/
	public event NextTurnHandler NextTurn = new NextTurnHandler(logNextTurn); //Contains subscribers to next turn method.
	public void Space_Key(){
		Debug.Log("Space Key Pressed!");
		currentPlayer++;
		if (currentPlayer >= NumberOfPlayers){
			currentPlayer = 0;
		}

		OnNextTurn(); // OnNext Turn Event fires.
	}

	private void OnNextTurn(){
		Debug.Log("OnNextTurn Event Firing!");
		if (NextTurn != null){
			NextTurn();
		}
	}

	static void logNextTurn(){
		Debug.Log("OnNextTurn Event Fired!");
	}
	#endregion
}
