using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ExitDoorLock : MonoBehaviour
{
    [SerializeField] private int unlocksNeeded= 5;
    private int[] locksReceived;
    private bool unlocked = false;

    //i need to add components on the door for it to work live

    private void Awake()
    {
        ExitSwitch[] switchesBuff = FindObjectsOfType<ExitSwitch>();
        for (int i = 0; i < switchesBuff.Length; i++) switchesBuff[i].selfIdx = i;
        locksReceived = new int[switchesBuff.Length];
    }

    public  bool TryToUnlockExit(int idx)
    {
        locksReceived[idx] = 1;

        float unlockCount = 0;

        foreach (int i in locksReceived) unlockCount += i;

        unlocked = Convert.ToBoolean( Mathf.Clamp(unlockCount - (unlocksNeeded-1), 0, 1) );
        transform.parent.GetChild(0).GetComponent<MeshRenderer>().enabled = unlocked;

        return unlocked;
    }
}
