using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModel : MonoBehaviour
{
	public GameObject hexPrefab;
	public GameObject[,] grid;
	public int width = 20;
	public int height = 20;
	public float spacing = 0f;
	public Player[] Players;

	//Hard coded values that reflect the dimensions of the hex tile blender file.
	private float hexHeight = 2f;
	private float hexWidth = 1.73205f;

	// Start is called before the first frame update
	void Start()
    {
		generateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void generateMap()
	{
		Quaternion rotation = Quaternion.Euler(-90,0,0);
		grid = new GameObject[width,height];

		//Orientation is Up-Down.
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
				float shift = 0;
				if (j % 2 == 0) {
					shift = (hexWidth + spacing) / 2;
				}
				float x = i;
				float z = j;
				Vector3 position = new Vector3((hexWidth + spacing) * x + shift, 0, ((hexHeight * 3f / 4f) + spacing) * z);
				grid[i, j] = Instantiate(hexPrefab, position, rotation);
				if (Random.value > 0.5) {
					grid[i, j].transform.GetComponent<HexSingleton>().Controller = Players[0]; // This is nasty. Isn't there another way?
					grid[i, j].transform.GetComponent<HexSingleton>().Name = "Player_1_Land";
				} else {
					grid[i, j].transform.GetComponent<HexSingleton>().Controller = Players[1];
					grid[i, j].transform.GetComponent<HexSingleton>().Name = "Player_2_Land";
				}
			}
		}
	}
}
