using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderInputs : MonoBehaviour
{
    public int particleCounts;
    public Material particleMaterial;
    public ComputeShader computeShader;

    public Color colorOne;
    public Color colorTwo;

    private void Awake()
    {
        if (particleMaterial.HasProperty("_Color1")) particleMaterial.SetColor("_Color1", colorOne);
        if (particleMaterial.HasProperty("_Color2")) particleMaterial.SetColor("_Color2", colorTwo);
    }
}
