using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPopulation : MonoBehaviour {

    [System.Serializable]
    public struct HexHouseData {
        public Transform houseLocation;
        public float populationCutoff;
        public GameObject prefab;
    };

    public HexHouseData[] houseData;

    private GameObject currentHouse = null;

    public void SetPopulation(float population) {
        Debug.Log("Set Population Called");
        if (currentHouse != null) {
            Destroy(currentHouse);
        }

        for (int n = houseData.Length - 1; n >= 0; --n) {
            if (population > houseData[n].populationCutoff) {
                currentHouse = Instantiate(houseData[n].prefab, houseData[n].houseLocation);
                break;
            }
        }
    }
}
