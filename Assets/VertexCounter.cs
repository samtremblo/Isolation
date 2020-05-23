using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexCounter : MonoBehaviour
{
    
    void Start()
    {
        MeshFilter[] allmesh = FindObjectsOfType<MeshFilter>();

        float maxVertex = 0;

        foreach(MeshFilter f in allmesh)
        {
            float count = f.mesh.vertexCount;
            print(f.gameObject.name + ": " + count + " vertices");
            if (count > maxVertex) maxVertex = count;
        }

        print("total max vertice count: " + maxVertex);
    }
}
