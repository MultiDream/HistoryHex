using System.Collections;
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
    public HistoryHex.StateMachine fsm;
    public HistoryHex.GameStates.PlayerTurn[] playerTurnStates;
    public HistoryHex.GameStates.Pause pauseState;
    public HistoryHex.GameStates.ConfirmExit confirmExit;
    public HistoryHex.GameStates.GameEnd gameEndState;
    public Map Board;               //Handles map creation.
	
	//Button Mappings;
	private KeyCode NextTurnKey = KeyCode.Space;
    private KeyCode PauseKey = KeyCode.Escape;

    private bool enableKeys = true;

    // Start is called before the first frame update
    void Start()
    {
		//Throws itself up into Globally Accessible Scope.
		Global.GM = this;

        enableKeys = true;

		Players = new GameObject[NumberOfPlayers];
        float split = 1.0f / NumberOfPlayers;
        float centeringOffset = split/2f;
        float rangeOffset = centeringOffset;

        Color[] teamColors =
        {
            Color.blue,
            Color.red
        };

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            float center = i*split + centeringOffset;
            Players[i] = Instantiate(playerPrefab);
            Players[i].transform.GetComponent<Player>().Colour = teamColors[i];// UnityEngine.Random.ColorHSV(center - rangeOffset, center + rangeOffset, 0.3f, 1f, 0.3f, 1f);
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

        UIMaster.instance.SetCurrentPlayerHUD();
    }

    // Update is called once per frame
    void Update()
    {
        if (enableKeys && Input.GetKeyDown(NextTurnKey))
        {
            NextTurnKeyPress();
        }

        if (enableKeys && Input.GetKeyDown(PauseKey)) {
            PauseKeyPress();
        }

        // TEMPORARY
        if (Input.GetKeyDown(KeyCode.Alpha8)) {
            GameEnd();
        }
	}

    void FixedUpdate()
    {
        //Board.DrawSelectedPath(UIMasterPrefab.GetComponent<UIMaster>().selectController);
    }

    public void GameEnd() {
        enableKeys = false;
        gameEndState.SetDisplayResults(Global.ActivePlayerId);
        playerTurnStates[Global.ActivePlayerId].OnGameEnd();
    }

    public void ExitGame() {
        pauseState.OnEndGamePressed();
    }

    public void ConfirmExitGame() {
        confirmExit.OnEndGamePressed();
    }

	/// <summary>
	/// Tests to see if end conditions have been met.
	/// If so, a playerId is given to represent the winner
	/// of the game. Else, return -1.
	/// </summary>
	/// <returns></returns>
	public void TestEndConditions(){
		HashSet<int> remainingPlayers = RemainingPlayers();
		if (remainingPlayers.Count <= 1) 
		{
			int winner = remainingPlayers.RemoveWhere(_ => true); //Removes first element in set.
			enableKeys = false;
			gameEndState.SetDisplayResults(winner);
			playerTurnStates[winner].OnGameEnd();
		} 
			
	}

	private HashSet<int> RemainingPlayers(){
		HashSet<int> remainingPlayers = new HashSet<int>();

		HexEntity entity;
		foreach (GameObject hexObj in Board.hexMap.Values){
			entity = hexObj.GetComponent<HexEntity>();
			remainingPlayers.Add(entity.Controller.PlayerId);
		}

		return remainingPlayers;
	}
    #region KeyBindings
    /*-------------------------------------------------
	 *                  Key Bindings
	 *-----------------------------------------------*/
    public void NextTurnKeyPress()
    {
        Debug.Log("Space Key Pressed!");
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

	public void PauseKeyPress()
	{
        if (fsm.GetCurrentState() == pauseState) {
            pauseState.OnReturnToGame(Global.ActivePlayerId);
        }
        else if (fsm.GetCurrentState() == confirmExit) {
            confirmExit.OnCancelPressed();
        }
        else {
            playerTurnStates[Global.ActivePlayerId].OnPause(pauseState);
        }
	}
	#endregion

	#region EventBindings

	public event NextTurnHandler NextTurn = new NextTurnHandler(LogNextTurn); //Contains subscribers to next turn method.
	private void OnNextTurn()
    {
		TestEndConditions();
		NextTurn(); // Event will never be null.
		HexUpdate();
		ArmyUpdate();
    }

	public event NextTurnHandler HexUpdate = new NextTurnHandler(LogNextTurn); //Contains subscribers to next turn method.
	private void OnHexUpdate() {
		HexUpdate(); // Event will never be null.
	}

	public event NextTurnHandler ArmyUpdate = new NextTurnHandler(LogNextTurn); //Contains subscribers to next turn method.
	private void OnArmyUpdate() {
		ArmyUpdate(); // Event will never be null.

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

	public event EndGameHandler EndGame = new EndGameHandler(LogEvent);
	private void OnEndGame(){
		EndGame(); // Event will never be null.
	}

	/// <summary>
	/// Default function for logging Events firing.
	/// </summary>
	static void LogEvent() {
		Debug.Log($"Event Fired!");
	}

	#endregion
}
