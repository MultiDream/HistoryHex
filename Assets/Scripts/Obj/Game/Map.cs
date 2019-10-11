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
    public GameObject hexPathPrefab;
    public Dictionary<Vector3Int, GameObject> hexMap; //Cubic is default.
    public AdjacencyMap adjacencyMap;
    public bool LabelHexes = false;
    private GameObject pathObject;
    private GameObject lastObject;

    public int radius = 5;

    //Hard coded values that reflect the dimensions of the hex tile blender file.
    // private float hexHeight = 2f;
    // private float hexWidth = 1.73205f;

    // Start is called before the first frame update
    void Start()
    {
        //InitMap();
        Global.MapFlyWeight = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //IMap functions.
    public void InitMap()
    {
        Dictionary<Vector3Int, GameObject> HeavyMap = new Dictionary<Vector3Int, GameObject>(new MapEqualityComparer());

        Vector3 Q = new Vector3(Mathf.PI / 3.0f, 0, 0.5f);      //  60* axis 
        Vector3 R = new Vector3(-Mathf.PI / 3.0f, 0, 0.5f);     // 120* axis
        Vector3 S = new Vector3(0, 0, -1);                      // 180* axis
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);      // Because the model is 90 degrees rotated.


        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                for (int k = -radius; k <= radius; k++)
                {
                    float gapChance = 0.125f;
                    if (i + j + k == 0 && Random.value < 1.0f - gapChance)
                    {

                        Vector3Int key = new Vector3Int(i, j, k);

                        // Beware order of operations.
                        Vector3 position = ((Q * i) + (R * j) + (S * k));
                        position = new Vector3(position.x * 0.85f, 0, position.z);


                        GameObject Hex = Instantiate(hexPrefab, position, rotation);
                        Hex.AddComponent<TextMesh>();
                        if (LabelHexes)
                        {
                            TextMesh text = Hex.GetComponent<TextMesh>();
                            text.text = "" + key;
                            text.characterSize = 0.1f;
                            text.fontSize = 32;
                            text.color = new Color(0, 0, 0);
                            text.offsetZ = 0.1f;
                            text.anchor = TextAnchor.MiddleCenter;
                        }

                        Hex.GetComponent<HexEntity>().Position = key;
                        HeavyMap.Add(key, Hex);
                    }
                }
            }
        }
        hexMap = HeavyMap;
        adjacencyMap = new AdjacencyMap();
        foreach (GameObject hex in hexMap.Values)
        {
            adjacencyMap.AddVertex(hex);
        }

    }

    public Dictionary<Vector3Int, GameObject> GetMapAxial()
    {
        Dictionary<Vector3Int, GameObject> axialMap = new Dictionary<Vector3Int, GameObject>();
        if (hexMap != null)
        {
            foreach (Vector3Int cubicKey in hexMap.Keys)
            {
                Vector3Int axialKey = new Vector3Int(cubicKey.x, cubicKey.y, cubicKey.z);
                axialMap.Add(axialKey, hexMap[cubicKey]);
            }
        }
        else
        {
            throw new UnityException("Map has not been initialized!");
        }

        return axialMap;
    }

    public void DrawSelectedPath(SelectController selectController)
    {
        Debug.Log("Ok! " + selectController.LastSelected + " 2nd " + selectController.SelectedObj);
        if (selectController.LastSelected != null && selectController.SelectedObj != null)
        {
            GameObject start = selectController.LastSelected.gameObject;
            GameObject finish = selectController.SelectedObj.gameObject;
            if (lastObject != finish && start.GetComponent<HexEntity>() != null && finish.GetComponent<HexEntity>() != null)
            {
                if (pathObject != null)
                {
                    pathObject.GetComponent<HexPath>().Destroy();
                    Destroy(pathObject);
                    pathObject = null;
                }
                lastObject = finish;
                pathObject = Instantiate(hexPathPrefab);
                HexPath path = pathObject.GetComponent<HexPath>();
                path.Initialize();
                path.DrawCircle(0.4f, 0.1f, start.transform.position.x, start.transform.position.z);
                path.DrawCircle(0.4f, 0.1f, finish.transform.position.x, finish.transform.position.z);
                //List<GameObject> vertices = adjacencyMap.RaggedWalk(start, finish, hexMap);
                //List<GameObject> vertices = adjacencyMap.RandomWalk(start, finish);
                //List<GameObject> vertices = adjacencyMap.DFS(start, finish);
                List<GameObject> vertices = adjacencyMap.NearestAstar(start, finish);

                path.AddHexes(vertices);
            }
        }
    }

    public Dictionary<Vector3Int, GameObject> GetMapCubic()
    {

        if (hexMap != null)
        {
            return hexMap;
        }
        else
        {
            throw new UnityException("Map has not been initialized!");
        }

    }

    // Checks the hexMap to see if this key is in it. Only works for cubic coordinates.
    public bool HasHexAtCubic(Vector3Int key)
    {
        return hexMap.ContainsKey(key);
    }

    public void setControl(Player[] players)
    {
        foreach (GameObject hexObj in hexMap.Values)
        {
            hexObj.GetComponent<HexEntity>().Controller = players[Mathf.FloorToInt(Random.value * players.Length)];
        }
    }

    // Grab a RandomHex from the list of world Hexes
    public GameObject RandomHex()
    {
        int size = hexMap.Count;
        int index = (int)Random.Range(0, size);
        int i = 0;
        foreach (GameObject value in hexMap.Values)
        {
            if (i == index)
                return value;
            i++;
        }
        return null;
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
