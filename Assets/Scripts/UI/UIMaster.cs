using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script's purpose is to manage a series of other UI components.
/// It should always be attached to a UIPrefab.
/// </summary>
public class UIMaster : MonoBehaviour
{
	/* We still need to decide the exact process of registering and unregistering UIComponents. */

	public GameMaster GM;
	public GameObject subComponent;
	public KeyboardController keyboard;			// Needs to be passed in.
	public SelectController selectController;	// Same deal.

	public TextMeshProUGUI currentPlayerText;
	public Image currentPlayerImage;

	public static UIMaster instance; // Just adding this bc idk how things are intended to be set up

    // Start is called before the first frame update
    void Start(){
		instance = this;
		
		BindKeys();
		keyboard.Listening = true;
		//Master needs to register to the GameMaster for NextTurnEvent!
		GM.NextTurn += new NextTurnHandler(Space_Key);
    }

    // Update is called once per frame
    void Update(){
    }

	// Register a UIComponent. Currently just does the flag.
	public void RegisterUIComponent(){
		if (subComponent == null){
			UIFactory factory = transform.GetComponent<UIFactory>();
			subComponent = factory.getUI();
			Color color = GM.Players[GM.currentPlayer].GetComponent<Player>().Colour;
			subComponent.transform.GetChild(0).GetComponent<Image>().color = color; //Jesus this is long. Clean later.
		}
	}

	//unregister a UIComponent
	public void UnregisterUIComponent(){
		Destroy(subComponent);
		subComponent = null;
	}

	// Key bindings
	void BindKeys(){
		keyboard.BindKey(KeyCode.F, F_Key);
		keyboard.BindKey(KeyCode.G, G_Key);
		keyboard.BindKey(KeyCode.J, J_Key);
		keyboard.BindKey(KeyCode.K, K_Key);
	}

	void F_Key() {
		Debug.Log("Using map mode 'FoodBase'. ");
		Global.CurrentMapMode = MapMode.Food;
	}

	void G_Key() {
		Debug.Log("Using map mode 'Controller'. ");
		Global.CurrentMapMode = MapMode.Controller;
	}

	void J_Key() {
		//Debug.Log("Registering UI");
		//RegisterUIComponent();
	}
	
	void K_Key() {
		Debug.Log("Unregistering UI");
		UnregisterUIComponent();
	}

	void Space_Key()
	{
		Debug.Log("UIMaster Notified of Space Hit!");
		SetCurrentPlayerHUD();
		if (subComponent != null){
			Color color = GM.Players[GM.currentPlayer].GetComponent<Player>().Colour;
			subComponent.transform.GetChild(0).GetComponent<Image>().color = color; //Jesus this is long. Clean later.
		}
		return;
	}

	public void SetCurrentPlayerHUD() {
		selectController.ClearSelected();
		
		Color color = GM.Players[GM.currentPlayer].GetComponent<Player>().Colour;
		color.a = 0.5f;
		currentPlayerImage.color = color;
		currentPlayerText.text = "Player " + GM.currentPlayer;
	}
}
