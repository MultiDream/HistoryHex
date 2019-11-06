using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class UIArmy : UICom
{
    [SerializeField] private TextMeshProUGUI armyText, playerText, foodText, populationText;
    [SerializeField] private Button moveButton, supplyButton;
    public override void SetText(string panelName, string player, string food, string foodGrowth, 
                    string population, string populationGrowth, string outFood, string outPopulation) {
        armyText.text = panelName;
        playerText.text = "Player " + player;
        foodText.text = food;
        populationText.text  = population;
    }

    public override void SetButtonListeners(params Action[] actions) {
        moveButton.onClick.AddListener(() => actions[0]());
        supplyButton.onClick.AddListener(() => actions[1]());
        return;
    }
}
