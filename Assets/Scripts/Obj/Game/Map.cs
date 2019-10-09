using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles the Creation and management of the Map.
/// Access using Flyweight in Global.
/// </summary>
public class Map : MonoBehaviour
{
	public GameObject hexPrefab;
	public Dictionary<int[],GameObject> hexMap; //Cubic is default.

	public int radius = 5;

	//Hard coded values that reflect the dimensions of the hex tile blender file.
	// private float hexHeight = 2f;
	// private float hexWidth = 1.73205f;

	// Start is called before the first frame update
	void Start() {
		//InitMap();
		Global.MapFlyWeight = this;
	}

	// Update is called once per frame
	void Update() {

	} 

	//IMap functions.
	public void InitMap(){
		Dictionary<int[], GameObject> HeavyMap = new Dictionary<int[], GameObject>(new MapEqualityComparer());

		Vector3 Q = new Vector3(Mathf.PI / 3.0f, 0, 0.5f);      //  60* axis 
		Vector3 R = new Vector3(-Mathf.PI / 3.0f, 0, 0.5f);     // 120* axis
		Vector3 S = new Vector3(0, 0, -1);                      // 180* axis
		Quaternion rotation = Quaternion.Euler(-90, 0, 0);      // Because the model is 90 degrees rotated.


		for (int i = -radius; i <= radius; i++) {
			for (int j = -radius; j <= radius; j++) {
				for (int k = -radius; k <= radius; k++) {
					float gapChance = 0.125f;
					if (i + j + k == 0 && Random.value < 1.0f - gapChance) {

						int[] key = new int[3] { i, j, k };

						// Beware order of operations.
						Vector3 position = ((Q * i) + (R * j) + (S * k));
						position = new Vector3(position.x * 0.85f,0,position.z);


						GameObject Hex = Instantiate(hexPrefab, position, rotation);
						Hex.GetComponent<HexEntity>().Position = key;
						HeavyMap.Add(key, Hex);
					}
				}
			}
		}
		hexMap = HeavyMap;
	}

	public Dictionary<int[], GameObject> GetMapAxial(){
		Dictionary<int[], GameObject> axialMap = new Dictionary<int[], GameObject>();
		if (hexMap != null) {
			foreach(int[] cubicKey in hexMap.Keys){
				int[] axialKey = new int[] { cubicKey[0], cubicKey[1]};
				axialMap.Add(axialKey,hexMap[cubicKey]);
			}
		} else {
			throw new UnityException("Map has not been initialized!");
		}

		return axialMap;
	}

	public Dictionary<int[], GameObject> GetMapCubic() {

		if (hexMap != null){
			return hexMap;
		} else {
			throw new UnityException("Map has not been initialized!");
		}

	}

	// Checks the hexMap to see if this key is in it. Only works for cubic coordinates.
	public bool HasHexAtCubic(int[] key){
		return hexMap.ContainsKey(key);
	}

	public void setControl(Player[] players){
		foreach (GameObject hexObj in hexMap.Values){
			hexObj.GetComponent<HexEntity>().Controller = players[Mathf.FloorToInt(Random.value * players.Length)];
		}
	}
}

// IEqualityConparator used to compare keys in the HexMap
// This code was developped primarily from this source:
// https://stackoverflow.com/questions/14663168/an-integer-array-as-a-key-for-dictionary
public class MapEqualityComparer : IEqualityComparer<int[]>
{
	public bool Equals(int[] x, int[] y) 
	{
		if (x.Length != y.Length) 
		{
			return false;
		}
		for (int i = 0; i < x.Length; i++) 
		{
			if (x[i] != y[i]) 
			{
				return false;
			}
		}
		return true;
	}

	public int GetHashCode(int[] key) 
	{
		// Will implode if you hit out of bounds exception.
		int result = 0;
		result = key[0] + key[1] * 32 + key[2] * 1024;
		return result;
	}
}
