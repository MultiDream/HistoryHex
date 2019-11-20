using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The home for player data, to be used by other controllers.
/// Pretty much just an entity. Consider renaming it.
/// </summary>
public class Player : MonoBehaviour
{
	public Color Colour = new Color(); //This is terrible naming. Won't somebody do something?

	public int PlayerId; //unique identifier. Please don't double up on ids. Please.

	public List<ArmyEntity> armies;

	public void Start() {
		armies = new List<ArmyEntity>();
	}

	public void Update() {
		
	}

	public void RefreshAllArmies(){
		foreach (ArmyEntity army in armies){
			army.RefreshSupplyLines();
		}
	}

	public void UnregisterAllMatchedOrders(HexEntity hex){
		foreach (ArmyEntity army in armies){
			foreach(HexPath path in army.supplyLines)
				path.UnRegisterMatchedOrder(hex);
		}
	}
}
