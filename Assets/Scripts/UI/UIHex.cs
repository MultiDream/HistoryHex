using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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

    public override void SetButtonListeners(params Action[] actions) {
		raiseArmyButton.onClick.AddListener(() => actions[0]());
		return;
    }
}
