using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SpritePaletteLookupGenerator))]
public class SpritePaletteLookupGeneratorEditor : Editor { 
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Palette"))
        {
            (target as SpritePaletteLookupGenerator).GenerateLookupTexture();
        }
    }
}