using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public Light[] lights;

    private bool lightsOn= false;


    public void SwitchLights()
    {
        lightsOn = !lightsOn;
        foreach (Light l in lights) l.enabled = lightsOn;
    }
}
