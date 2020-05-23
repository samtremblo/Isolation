using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;


public class flashlightControl : MonoBehaviour
{
    private Light myLight;

    // Start is called before the first frame update
    void Start()
    {
      myLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Actions._default.Teleport.GetLastStateUp(SteamVR_Input_Sources.Any))
        {
            if (myLight.enabled)
            {
                myLight.enabled = false;

            }
            else if(!myLight.enabled)
            {
                myLight.enabled = true;

            }
        }
        
    }
}
