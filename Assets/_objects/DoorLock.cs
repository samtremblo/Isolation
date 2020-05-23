using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private DoorLock secondDoorInRoom;
    private Rigidbody rigid;
    private Transform handle;
    private Quaternion originRot;
    private Agent agent;
    private SceneLoading sceneLoading;

    private DoorLock[] otherDoorLocks;

    private bool holding = false;
    private bool doorIsOpen = false;

    private bool firstTime = true;
    public bool FirstTimeOpen
    {
        get { return firstTime; }
        set { firstTime = value; }
    }
    //private float openedTime;

    private void Awake()
    {
        agent = FindObjectOfType<Agent>();
        originRot = transform.rotation;
        rigid = GetComponent<Rigidbody>();
        sceneLoading = FindObjectOfType<SceneLoading>();
        otherDoorLocks = FindObjectsOfType<DoorLock>();
    }

    /*************************************************************************/
    public void GrabbingDoor()
    {
        doorIsOpen = true;
        sceneLoading.LoadThisScene(gameObject.name);
        if (FirstTimeOpen) StartCoroutine(TargetDoorAsAgentSpawn());
    }
    public void ReleasingDoor()
    {
        holding = false;
        StartCoroutine(CheckForDoor());
    }

    /*************************************************************************/

    private IEnumerator TargetDoorAsAgentSpawn()
    {
        foreach (DoorLock dl in otherDoorLocks) dl.FirstTimeOpen = false;
        FirstTimeOpen = false;
        while (Quaternion.Angle(transform.rotation, originRot) < 10)
        {
            yield return null;
        }
        Vector3 pos = transform.parent.parent.position - transform.parent.parent.right* 1;
        pos.y = agent.Origin.y;
        agent.InitialSpook(pos);
    }

    private IEnumerator CheckForDoor()
    {
        while(Quaternion.Angle(transform.rotation, originRot) > 0.1)
        {
            yield return null;
        }

        rigid.angularVelocity = Vector3.zero;
        rigid.velocity = Vector3.zero;
        rigid.rotation = originRot;
        doorIsOpen = false;
        if (secondDoorInRoom == null || secondDoorInRoom.doorIsOpen == false)
            sceneLoading.UnloadThisScene(gameObject.name);
    }
}
