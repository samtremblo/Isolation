using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadTest : MonoBehaviour
{
    List<MeshRenderer> allmeshes= new List<MeshRenderer>();

    public bool disableMesh = false;
    private bool lastBool= false;

    private void Awake()
    {
        Transform[] go = GameObject.FindObjectsOfType(typeof(Transform)) as Transform[];
        for(int i=0; i< go.Length; i++)
        {
            if (go[i].GetComponent<MeshFilter>() != null)
                allmeshes.Add(go[i].GetComponent<MeshRenderer>());
        }
    }

    private void Update()
    {
        if(disableMesh && !lastBool)
        {
            foreach (MeshRenderer m in allmeshes) m.enabled = false;
            lastBool = true;
        }            
        else if (!disableMesh && lastBool)
        {
            foreach (MeshRenderer m in allmeshes) m.enabled = true;
            lastBool = false;
        }
            
    }
}
