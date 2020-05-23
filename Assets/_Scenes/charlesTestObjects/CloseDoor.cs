using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoor : MonoBehaviour
{
    private OpenDoor opendoor;

    private void Awake()
    {
        opendoor = transform.parent.GetComponentInChildren<OpenDoor>();//GetComponentInParent<OpenDoor>();
        print(opendoor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "HeadCollider")
        {
            opendoor.OnClosedDoor();
        }
    }
}
