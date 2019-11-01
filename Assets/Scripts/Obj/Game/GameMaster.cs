using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;


// Relevant Delegates:
public delegate void NextTurnHandler(); //Handles moving game forward one turn.
public delegate void NextCycleHandler(); //Handles restarting the player cycle.

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
    public Map Board;               //Handles map creation.

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

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Space_Key();
        }
    }

    void FixedUpdate()
    {
        //Board.DrawSelectedPath(UIMasterPrefab.GetComponent<UIMaster>().selectController);
    }

    #region KeyBindings
    /*-------------------------------------------------
	 *                  Key Bindings
	 *-----------------------------------------------*/
	public void Space_Key()
    {
        Debug.Log("Space Key Pressed!");
        currentPlayer++;
        if (currentPlayer >= NumberOfPlayers)
        {
            currentPlayer = 0;
			OnNextCycle();
        }
        Global.ActivePlayerId = Players[currentPlayer].GetComponent<Player>().PlayerId;

        OnNextTurn();
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
	#endregion
}
