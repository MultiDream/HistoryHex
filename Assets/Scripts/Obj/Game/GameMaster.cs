﻿using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;


// Relevant Delegates:
public delegate void NextTurnHandler(); //Handles moving game forward one turn.
public delegate void NextCycleHandler(); //Handles restarting the player cycle.
public delegate void EndGameHandler();

/// <summary>
/// Master Class for the game. Holds important logic and classes for intializing
/// and observing the state of the game.
/// </summary>

public class GameMaster : MonoBehaviour
{

    //Prefabs needed for this Component and sub components.
    public GameObject playerPrefab;
    public GameObject UIMasterPrefab;
    public int NumberOfPlayers;
    public int currentPlayer = 0;
    public GameObject[] Players;
    public HistoryHex.GameStates.PlayerTurn[] playerTurnStates;
    public Map Board;               //Handles map creation.
	
	//Button Mappings;
	private KeyCode NextTurnKey = KeyCode.Space;
	private KeyCode SurrenderKey = KeyCode.Escape;

	// Start is called before the first frame update
	void Start()
    {
		//Throws itself up into Globally Accessible Scope.
		Global.GM = this;

		Players = new GameObject[NumberOfPlayers];
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            Players[i] = Instantiate(playerPrefab);
            Players[i].transform.GetComponent<Player>().Colour = UnityEngine.Random.ColorHSV();
            Players[i].transform.GetComponent<Player>().PlayerId = i;
        }

        Board.InitMap();

        Player[] _players = new Player[NumberOfPlayers];
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            _players[i] = Players[i].GetComponent<Player>();
        }

        //Possible to refactor by tossing current player into the Global flyweight.
        Board.setControl(_players); //Needs to run after the map is generated.
        Board.InitPlayerAdjacencies();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(NextTurnKey))
        {
            NextTurnKeyPress();
        }

<<<<<<< Updated upstream
		if (Input.GetKeyDown(SurrenderKey)) {
			SurrenderKeyPress();
		}
	}
=======
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GameEnd();
        }
    }
>>>>>>> Stashed changes

    void FixedUpdate()
    {
        //Board.DrawSelectedPath(UIMasterPrefab.GetComponent<UIMaster>().selectController);
    }

    public void GameEnd() {
        playerTurnStates[Global.ActivePlayerId].OnGameEnd();
    }

    #region KeyBindings
    /*-------------------------------------------------
	 *                  Key Bindings
	 *-----------------------------------------------*/
	public void NextTurnKeyPress()
    {
        //Debug.Log("Space Key Pressed!");
        playerTurnStates[Global.ActivePlayerId].OnTurnEnd();
        currentPlayer++;
        if (currentPlayer >= NumberOfPlayers)
        {
            currentPlayer = 0;
			OnNextCycle();
        }
        Global.ActivePlayerId = Players[currentPlayer].GetComponent<Player>().PlayerId;

        OnNextTurn();
    }

	public void SurrenderKeyPress()
	{
		//if (SurrenderPrompt()){ Prompt player for surrender.

		Debug.Log("Player " + currentPlayer + " has surrendered.");
	}
	#endregion

	#region EventBindings

	public event NextTurnHandler NextTurn = new NextTurnHandler(LogNextTurn); //Contains subscribers to next turn method.
	private void OnNextTurn()
    {
		NextTurn(); // Event will never be null.
    }

	/// <summary>
	/// Default function for logging next Turn Events firing.
	/// </summary>
    static void LogNextTurn()
    {
        Debug.Log("OnNextTurn Event Fired!");
    }

	public event NextCycleHandler NextCycle = new NextCycleHandler(LogNextCycle); //Contains subscribers to next turn method.
	private void OnNextCycle() {
		NextCycle(); // Event will never be null.
	}
	/// <summary>
	/// Default function for logging next Cycle Events firing.
	/// </summary>
	static void LogNextCycle() {
		Debug.Log("OnNextCycle Event Fired!");
	}

	public event EndGameHandler EndGame = new EndGameHandler(LogEndGame);
	private void OnEndGame(){
		EndGame(); // Event will never be null.
	}
	/// <summary>
	/// Default function for logging next Cycle Events firing.
	/// </summary>
	static void LogEndGame() {
		Debug.Log("OnNextCycle Event Fired!");
	}


	#endregion
}
