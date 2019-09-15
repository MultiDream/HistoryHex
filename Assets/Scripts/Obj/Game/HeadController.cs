using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Master Class for the game.
/// </summary>
public class HeadController : MonoBehaviour
{
	public GameObject[] playerPrefabs;
	public GameObject UI;

	public int activePlayer = 0;
	public KeyboardController keyboard;
    // Start is called before the first frame update
    void Start()
    {
		keyboard.BindKey(KeyCode.K,K_Press);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	// Key Bindings

	/// <summary>
	/// Changes the active player.
	/// </summary>
	public void K_Press(){
		Debug.Log("K Key Pressed!");
		if (activePlayer < playerPrefabs.Length - 1){
			activePlayer++;
		} else {
			activePlayer = 0;
		}

		//Set UI flag color.
		UI.GetComponent<UIMaster>().ActivePlayer = playerPrefabs[activePlayer].GetComponent<Player>();
		return;
	}
}
