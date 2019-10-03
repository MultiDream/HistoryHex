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
    private List<HexEntity> hexEntities; // list of tiles
    private Dictionary<CompositeKey<HexEntity, HexEntity>, bool> adjacencyMap;
    //Prefabs
    //Public variables
    public string Name { get; set; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
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
    private void Initialize()
    {
        Name = "EmptyPath";
        hexEntities = new List<HexEntity>();
        adjacencyMap = new Dictionary<CompositeKey<HexEntity, HexEntity>, bool>();
        adjacencyMap[new CompositeKey<HexEntity, HexEntity>(null, null)] = false;
    }


    // ContainsHex - wraps the list Contains function.
    public bool ContainsHex(HexEntity hex)
    {
        return hexEntities.Contains(hex);
    }

    private CompositeKey<HexEntity, HexEntity> GetAdjacencyKey(HexEntity hex1, HexEntity hex2)
    {
        CompositeKey<HexEntity, HexEntity> key12 = new CompositeKey<HexEntity, HexEntity>(hex1, hex2);
        CompositeKey<HexEntity, HexEntity> key21 = new CompositeKey<HexEntity, HexEntity>(hex2, hex1);
        if (adjacencyMap.ContainsKey(key21))
            return key21;
        else
            return key12;
    }

    private CompositeKey<HexEntity, HexEntity> GetExistingAdjacencyKey(HexEntity hex1, HexEntity hex2)
    {
        CompositeKey<HexEntity, HexEntity> key = GetAdjacencyKey(hex1, hex2);
        if (adjacencyMap.ContainsKey(key))
            return key;
        return new CompositeKey<HexEntity, HexEntity>(null, null);
    }

    // Update adjacency matrix
    private void UpdateAdjacency(HexEntity hex1, HexEntity hex2)
    {
        CompositeKey<HexEntity, HexEntity> key = GetAdjacencyKey(hex1, hex2);        
        adjacencyMap.Add(key, hex1.Adjacent(hex2));
    }

    // Update adjacency matrix with existing hexes
    private void UpdateAdjacency(HexEntity hex1)
    {
        foreach (HexEntity hex2 in hexEntities)
        {
            if (hex1 == hex2)
                continue;
            UpdateAdjacency(hex1, hex2);
        }
    }

    // Update all entries in adjacency matrix (AVOID USING)
    private void UpdateAllAdjacency()
    {
        foreach (HexEntity hex1 in hexEntities)
        {
            UpdateAdjacency(hex1);
        }
    }

    private void UpdateExistingAdjacency(HexEntity hex1, HexEntity hex2)
    {
        CompositeKey<HexEntity, HexEntity> key = GetExistingAdjacencyKey(hex1, hex2);
        if (key.IsNull())
            adjacencyMap.Add(key, hex1.Adjacent(hex2));
    }

    private void RemoveAdjacency(HexEntity hex1, HexEntity hex2)
    {
       CompositeKey<HexEntity, HexEntity> key = GetExistingAdjacencyKey(hex1, hex2);
       if (key.IsNull())
        {
            adjacencyMap.Remove(key);
        }
    }
    public bool Adjacent(HexEntity hex1, HexEntity hex2)
    {
        CompositeKey<HexEntity, HexEntity> key = GetExistingAdjacencyKey(hex1, hex2);
        return adjacencyMap[key];
    }

    // AddHex - public function for adding Hex to the path.
    //             returns True if Hex was added, or already exists in list.
    //             Wraps list adder, and updates adjacency list.
    public bool AddHex(HexEntity hex)
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


}