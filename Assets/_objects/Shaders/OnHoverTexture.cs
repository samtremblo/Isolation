using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class OnHoverTexture : MonoBehaviour
{
    public bool manualActivation;
    public float hoverActiveSpeed = 1;

    Interactable interactionScript;
    List<Material> mats= new List<Material>();
    string hoverEffect = "Vector1_69E3E6B6";
    float hoverValue = 0;
    float lastValue = 0; //reduce calls to shader

    void Start()
    {
        interactionScript = GetComponent<Interactable>(); //get interaction script

        Renderer[] buffRend = GetComponentsInChildren<Renderer>(); //find material, but only the ones ready for hover effect
        for(int i=0; i<buffRend.Length; i++)
        {
            if ( buffRend[i].material.HasProperty(hoverEffect) ) mats.Add(buffRend[i].material);
        }
        //if (mats.Count == 0 && GetComponent<Renderer>().material.HasProperty(hoverEffect)) mats.Add(GetComponent<Renderer>().material); //if the script has no child or was put on the material
    }

    void Update()
    {
        if(mats.Count > 0)
        {
            if (manualActivation || interactionScript.isHovering) // smooth incrementation,just set hoveractivespeed to 1 if not needed.
            {
                //print(hoverValue);
                if (hoverValue < 1) hoverValue += hoverActiveSpeed;
                if (hoverValue > 1) hoverValue = 1;
            }
            else if (hoverValue > 0)
            {
                hoverValue -= hoverActiveSpeed;
                if (hoverValue < 0) hoverValue = 0;
            }

            if (lastValue != hoverValue)
            {
                foreach (Material m in mats) m.SetFloat(hoverEffect, hoverValue);
                lastValue = hoverValue;
            }
        }
        

    }
}
