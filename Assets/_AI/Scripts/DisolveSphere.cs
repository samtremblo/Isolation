using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisolveSphere : MonoBehaviour
{
    private DisolveParticles disolver;
    [HideInInspector] public bool destroy=false;

    private void Awake()
    {
        disolver = FindObjectOfType<DisolveParticles>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destroy && other.tag == "Disolve_Mesh")
        {
            MeshFilter objMesh = other.GetComponent<MeshFilter>();
            print(objMesh);
            if (objMesh != null) disolver.TargetObjectToDestroy(objMesh);
        }       
    }
}
