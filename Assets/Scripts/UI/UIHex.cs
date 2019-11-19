using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;// Required when using Event data.
using TMPro;

public class UIHex : UICom
{
    [SerializeField] private TextMeshProUGUI villageText, playerText, foodText, foodGrowthText,
                         populationText, populationGrowthText, outFoodText, outPopulationText;
	[SerializeField] private Button raiseArmyButton;

	public override void SetText(string panelName, string player, string food, string foodGrowth, 
                    string population, string populationGrowth, string outFood, string outPopulation) {
        villageText.text = panelName;
        playerText.text = "Player " + player;
        foodText.text = food;
        foodGrowthText.text = "( +" + foodGrowth + ")";
        populationText.text  = population;
        populationGrowthText.text = "( +" + populationGrowth + ")";
        outFoodText.text = outFood;
        outPopulationText.text = outPopulation;
    }

	public void AllowRaiseArmy(){
		ColorBlock colors = raiseArmyButton.colors;
		colors.highlightedColor = Color.yellow;
		colors.normalColor = Color.white;
		colors.selectedColor = Color.white;
		colors.pressedColor = Color.grey;

		raiseArmyButton.colors = colors;
	}

	public void DenyRaiseArmy() {
		ColorBlock colors = raiseArmyButton.colors;
		colors.highlightedColor = Color.grey;
		colors.normalColor = Color.grey;
		colors.pressedColor = Color.grey;
		colors.selectedColor = Color.grey;

		raiseArmyButton.colors = colors;
	}

	public override void SetButtonListeners(params Action[] actions) {
		raiseArmyButton.onClick.AddListener(() => actions[0]());
		return;
    }
}
