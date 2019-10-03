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
	public Dictionary<Vector3Int,GameObject> hexMap; //Cubic is default.

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
		Dictionary<Vector3Int, GameObject> HeavyMap = new Dictionary<Vector3Int, GameObject>(new MapEqualityComparer());

		Vector3 Q = new Vector3(Mathf.PI / 3.0f, 0, 0.5f);      //  60* axis 
		Vector3 R = new Vector3(-Mathf.PI / 3.0f, 0, 0.5f);     // 120* axis
		Vector3 S = new Vector3(0, 0, -1);                      // 180* axis
		Quaternion rotation = Quaternion.Euler(-90, 0, 0);      // Because the model is 90 degrees rotated.


		for (int i = -radius; i <= radius; i++) {
			for (int j = -radius; j <= radius; j++) {
				for (int k = -radius; k <= radius; k++) {
					float gapChance = 0.125f;
					if (i + j + k == 0 && Random.value < 1.0f - gapChance) {

						Vector3Int key = new Vector3Int(i, j, k);

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

	public Dictionary<Vector3Int, GameObject> GetMapAxial(){
		Dictionary<Vector3Int, GameObject> axialMap = new Dictionary<Vector3Int, GameObject>();
		if (hexMap != null) {
			foreach(Vector3Int cubicKey in hexMap.Keys){
				Vector3Int axialKey = new Vector3Int(cubicKey.x, cubicKey.y, cubicKey.z);
				axialMap.Add(axialKey,hexMap[cubicKey]);
			}
		} else {
			throw new UnityException("Map has not been initialized!");
		}

		return axialMap;
	}

	public Dictionary<Vector3Int, GameObject> GetMapCubic() {

		if (hexMap != null){
			return hexMap;
		} else {
			throw new UnityException("Map has not been initialized!");
		}

	}

	// Checks the hexMap to see if this key is in it. Only works for cubic coordinates.
	public bool HasHexAtCubic(Vector3Int key){
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
public class MapEqualityComparer : IEqualityComparer<Vector3Int>
{
	public bool Equals(Vector3Int x, Vector3Int y) 
	{		
		return x.x == y.x && x.y == y.y && x.z == y.z;
	}

	public int GetHashCode(Vector3Int key) 
	{
		// Will implode if you hit out of bounds exception.
		int result = 0;
		result = key.x + key.y * 32 + key.z * 1024;
		return result;
	}
}
