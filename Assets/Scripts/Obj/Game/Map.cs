using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles the Creation and management of the Map.
/// </summary>
public class Map : MonoBehaviour
{
	public GameObject hexPrefab;
	private Dictionary<int[],GameObject> hexMap; //Cubic is default.

	public int radius = 5;

	//Hard coded values that reflect the dimensions of the hex tile blender file.
	private float hexHeight = 2f;
	private float hexWidth = 1.73205f;

	// Start is called before the first frame update
	void Start() {
		InitMap();
	}

	// Update is called once per frame
	void Update() {

	}

	//IMap functions.
	public void InitMap(){
		Dictionary<int[], GameObject> HeavyMap = new Dictionary<int[], GameObject>();

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
}
