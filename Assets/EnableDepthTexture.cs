using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnableDepthTexture : MonoBehaviour {
    private void Start() {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}