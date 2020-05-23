using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerLock : MonoBehaviour
{
    private Rigidbody rigid;
    private Quaternion originRot;
    private bool open = false;
    private float openedTime;

    private void Awake()
    {
        originRot = transform.rotation;
        rigid = GetComponent<Rigidbody>();
        //hinge.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        if (open)
        {
            if (Quaternion.Angle(transform.rotation, originRot) < 0.1 && Time.time - openedTime > 1)
            {
                open = false;
                rigid.angularVelocity = Vector3.zero;
                rigid.velocity = Vector3.zero;
                rigid.rotation = originRot;
            }
        }
    }

    public void UnlockDoor()
    {
        open = true;
        openedTime = Time.time;
    }
}
