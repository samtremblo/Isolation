using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitBit_head : MonoBehaviour
{
    public FitBit fitBit;


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "room1" || other.name == "room2" || other.name == "room3" || other.name == "room4" || other.name == "room5" || other.name == "room6")
            fitBit.ChangeColorMask(other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "room1" || other.name == "room2" || other.name == "room3" || other.name == "room4" || other.name == "room5" || other.name == "room6")
            fitBit.ChangeColorMask("hall");
    }
}
