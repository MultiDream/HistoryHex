using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsValues : MonoBehaviour
{
    
    public Scrollbar mapScrollBar;
    private int mapSize = 5;
    private float lastMapValue = 0;

    void OnEnable() {
        mapScrollBar.onValueChanged.AddListener(scrollbarCallBack);
        lastMapValue = mapScrollBar.value;
    }

    void scrollbarCallBack(float value) {
        if (lastMapValue > value) {
            UnityEngine.Debug.Log("Scrolling Right: " + value);
        } else {
            UnityEngine.Debug.Log("Scrolling Left: " + value);
        }
        lastMapValue = value;
    }

    void OnDisable() {
        mapScrollBar.onValueChanged.RemoveListener(scrollbarCallBack);
    }
}
