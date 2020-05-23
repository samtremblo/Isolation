using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRelays : MonoBehaviour
{
    [HideInInspector] public Brain_Audition brainAudioProcess;

    public void ObjectTrown(Rigidbody _rigid)
    {
        if(_rigid.velocity.magnitude > 0.1f)
            brainAudioProcess.StartTracking(_rigid);
        else
            brainAudioProcess.StartTracking(null);
    }
}
