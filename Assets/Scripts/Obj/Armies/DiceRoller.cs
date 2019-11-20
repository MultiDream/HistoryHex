using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DiceRoller{
    public int sides = 6;
    public DiceRoller(int sides=6){
        this.sides = sides;
    }

	public int Roll(){
		return Random.Range(1,this.sides);
	}

    public List<int> Roll(int n){
        List<int> rolls = new List<int>();
        for (int i = 0; i<n;i++)
            rolls.Add(Roll());
        return rolls;
    }


}