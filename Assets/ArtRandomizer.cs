using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ArtRandomizer : MonoBehaviour
{

    [SerializeField] private MeshFilter hexBase;
    [SerializeField] private Renderer hexTop;
    public Mesh[] BaseMeshes;
    public Material[] TopMaterials;
    public float[] MaterialWeights;

    private void OnValidate()
    {
        if (TopMaterials.Length != MaterialWeights.Length) {
            MaterialWeights = new float[TopMaterials.Length];
            for (int i = 0; i < MaterialWeights.Length; i++) {
                MaterialWeights[i] = 1f / MaterialWeights.Length;
            }
        }
    }

    void Start()
    {
        hexBase.mesh = BaseMeshes[Random.Range(0, BaseMeshes.Length)];
        float rand = Random.Range(0f, 1f);
        for (int i = 0; i < MaterialWeights.Length; i++) {
            if (rand < MaterialWeights[i]) {
                hexTop.material = TopMaterials[i];
                break;
            }
            rand -= MaterialWeights[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
