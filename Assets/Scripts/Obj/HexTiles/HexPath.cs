using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
ClassName:      HexPath
Author:         Brad Baker
Creation Date:  10032019
Inherits:       MonoBehavior ?
Abstract:       No
Brief:          An object for creating paths of Hex tiles. Convenient storage of references to hex tiles,
                    as well as additional properties and convenient methods for checking breaks,
                    and finding shortest paths.
Constructor:    HexPath
*/
public class HexPath : MonoBehaviour
{ 
    #region Properties
    // Internal variables
    private List<GameObject> hexEntities; // list of tiles
    private Dictionary<CompositeKey<GameObject, GameObject>, bool> adjacencyMap;
    //Prefabs
    //Public variables
    public string Name { get; set; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Draw();
    }

    // Update is called once per frame.
    void Update()
    {

    }

    //If Activated, run the extended activation methods.
    private void ActiveUpdate()
    {

    }

    private void WireSelectionInterface()
    {

    }

    //Start Delegation
    public void Initialize()
    {
        Debug.Log("Initializing!");
        Name = "EmptyPath";
        hexEntities = new List<GameObject>();
        adjacencyMap = new Dictionary<CompositeKey<GameObject, GameObject>, bool>();
        adjacencyMap[new CompositeKey<GameObject, GameObject>(null, null)] = false;
    }


    // ContainsHex - wraps the list Contains function.
    public bool ContainsHex(GameObject hex)
    {
        return hexEntities.Contains(hex);
    }

    private CompositeKey<GameObject, GameObject> GetAdjacencyKey(GameObject hex1, GameObject hex2)
    {
        CompositeKey<GameObject, GameObject> key12 = new CompositeKey<GameObject, GameObject>(hex1, hex2);
        CompositeKey<GameObject, GameObject> key21 = new CompositeKey<GameObject, GameObject>(hex2, hex1);
        if (adjacencyMap.ContainsKey(key21))
            return key21;
        else
            return key12;
    }

    private CompositeKey<GameObject, GameObject> GetExistingAdjacencyKey(GameObject hex1, GameObject hex2)
    {
        CompositeKey<GameObject, GameObject> key = GetAdjacencyKey(hex1, hex2);
        if (adjacencyMap.ContainsKey(key))
            return key;
        return new CompositeKey<GameObject, GameObject>(null, null);
    }

    // Update adjacency matrix
    private void UpdateAdjacency(GameObject hex1, GameObject hex2)
    {
        CompositeKey<GameObject, GameObject> key = GetAdjacencyKey(hex1, hex2);        
        adjacencyMap.Add(key, hex1.GetComponent<HexEntity>().Adjacent(hex2.GetComponent<HexEntity>()));
    }

    // Update adjacency matrix with existing hexes
    private void UpdateAdjacency(GameObject hex1)
    {
        foreach (GameObject hex2 in hexEntities)
        {
            if (hex1 == hex2)
                continue;
            UpdateAdjacency(hex1, hex2);
        }
    }

    // Update all entries in adjacency matrix (AVOID USING)
    private void UpdateAllAdjacency()
    {
        foreach (GameObject hex1 in hexEntities)
        {
            UpdateAdjacency(hex1);
        }
    }

    private void UpdateExistingAdjacency(GameObject hex1, GameObject hex2)
    {
        CompositeKey<GameObject, GameObject> key = GetExistingAdjacencyKey(hex1, hex2);
        if (key.IsNull())
            adjacencyMap.Add(key, hex1.GetComponent<HexEntity>().Adjacent(hex2.GetComponent<HexEntity>()));
    }

    private void RemoveAdjacency(GameObject hex1, GameObject hex2)
    {
       CompositeKey<GameObject, GameObject> key = GetExistingAdjacencyKey(hex1, hex2);
       if (key.IsNull())
        {
            adjacencyMap.Remove(key);
        }
    }
    public bool Adjacent(GameObject hex1, GameObject hex2)
    {
        CompositeKey<GameObject, GameObject> key = GetExistingAdjacencyKey(hex1, hex2);
        return adjacencyMap[key];
    }

    // AddHex - public function for adding Hex to the path.
    //             returns True if Hex was added, or already exists in list.
    //             Wraps list adder, and updates adjacency list.
    public bool AddHex(GameObject hex)
    {
        if (!ContainsHex(hex))
        {
            hexEntities.Add(hex);
            UpdateAdjacency(hex);
        }
        return ContainsHex(hex);
    }

    public bool IsConnected()
    {
        return false;
    }

    private void DrawSegment(Vector3 start, Vector3 end, Color color)
    {
        start += new Vector3(0, 0.1f, 0);
        end += new Vector3(0, 0.1f, 0);
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
    }

    private void Draw(){
        GameObject hex1, hex2;
        Color pathColor = UnityEngine.Random.ColorHSV();
        for (int i = 0; i < hexEntities.Count - 1; i++){
            hex1 = hexEntities[i];
            if (i + 1 >= hexEntities.Count){
                break;
            }
            hex2 = hexEntities[i+1];
            DrawSegment(hex1.transform.position, hex2.transform.position, pathColor);
        }
    }
    
}