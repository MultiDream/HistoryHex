using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a number of global project settings.
/// </summary>
public static class Global
{
    // Global variables.
    public static float MAXIMUM_FOOD = 4.0f;
    public static int MAX_PLAYERS = 4;

    //Map Flyweight and related tools.
    public static Map MapFlyWeight;
    public static Vector3 Q = new Vector3(Mathf.PI / 3.0f, 0, 0.5f);      //  60* axis 
    public static Vector3 R = new Vector3(-Mathf.PI / 3.0f, 0, 0.5f);     // 120* axis
    public static Vector3 S = new Vector3(0, 0, -1);                      // 180* axis

    public static Vector3 GetCubicVector(int i, int j, int k)
    {
        Vector3 position = ((Q * i) + (R * j) + (S * k));
        position = new Vector3(position.x * 0.85f, 0, position.z);
        return position;
    }

    //Current Player Flyweight and related tools.
    public static int ActivePlayerId;

    public static AdjacencyMap globalAdjacency;

    public static bool debugPath = false;
    public static bool labelHexes = false;

    //MapMode
    public static MapMode CurrentMapMode = MapMode.Food;
}

public enum MapMode
{
    Food = 0,
    Controller = 1
}
