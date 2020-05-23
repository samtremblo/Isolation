using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSwitch : MonoBehaviour
{
    [HideInInspector] public int selfIdx;

    public void OnSwitchTrigger()
    {
        FindObjectOfType<ExitDoorLock>().TryToUnlockExit(selfIdx);
    }

}
