using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class OpenDoor : MonoBehaviour
{
    public float openRot;
    public float initialSpeed;
    private float closedRot;
    private bool C_isrunning = false;
    private Transform doorHandle;
    private Coroutine action;
    private bool isGrab = false;
    private CircularDrive circDrive;

    private void Awake()
    {
        closedRot = transform.rotation.y;
        doorHandle = transform.GetChild(0).GetChild(0);
        circDrive = doorHandle.GetComponent<CircularDrive>();

    }

    private void Update()
    {
        isGrab = circDrive.isGrab;
    }

    public void OnOpenDoor()
    {
        if (action != null)
        {
            StopCoroutine(action);
            C_isrunning = false;
        }
        action = StartCoroutine(openDoor());
        print("open door");
    }

    public void OnClosedDoor()
    {
        if (action != null)
        {
            StopCoroutine(action);
            C_isrunning = false;
        }
        action = StartCoroutine(closedDoor());
    }



    private IEnumerator openDoor()
    {
        //yield return new WaitForSeconds(0.5f);
        while (isGrab)
        {
            yield return null;
        }
        C_isrunning = true;
        float angle = Quaternion.Angle(transform.rotation , Quaternion.Euler(0, openRot, 0));
        float maxAngle = angle;

        while (Mathf.Abs(angle) > 1f)
        {
            doorHandle.transform.localRotation = Quaternion.RotateTowards(doorHandle.transform.localRotation, Quaternion.Euler(0,0,0), 10*Time.deltaTime); 

            angle= Quaternion.Angle(transform.rotation, Quaternion.Euler(0, openRot, 0));

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,openRot,0),  Mathf.Max((angle/ maxAngle) , 0.05f)* initialSpeed * Time.deltaTime);

            yield return null;
        }
        C_isrunning = false;
        yield return null;
    }

    private IEnumerator closedDoor()
    {
        print("closeDoor");
        C_isrunning = true;
        float angle = Quaternion.Angle(transform.rotation, Quaternion.Euler(0, closedRot, 0));
        float maxAngle = angle;

        while (Mathf.Abs(angle) > 1f)
        {
            angle = Quaternion.Angle(transform.rotation, Quaternion.Euler(0, closedRot, 0));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, closedRot, 0), Mathf.Max((angle / maxAngle), 0.05f) * initialSpeed * Time.deltaTime);
            yield return null;
        }
        C_isrunning = false;
        yield return null;
    }
}
