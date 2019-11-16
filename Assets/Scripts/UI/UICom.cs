using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UICom : MonoBehaviour
{
    
    public abstract void SetText(string panelName, string player, string food, string foodGrowth, 
                    string population, string populationGrowth, string outFood, string outPopulation);

    public abstract void SetButtonListeners(params Action[] actions);

}
