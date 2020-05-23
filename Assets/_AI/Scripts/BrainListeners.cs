using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class BrainListeners : MonoBehaviour
{
    Brain_Audition audioBrain;
    ComplexThrowable[] throwables;

    private void Awake()
    {
        audioBrain = FindObjectOfType<Brain_Audition>();
        throwables = GetComponentsInChildren<ComplexThrowable>();
    }

    void Start()
    {
        foreach(ComplexThrowable th in throwables)
        {
            Rigidbody rigid = th.GetComponent<Rigidbody>();
            th.onDetach.AddListener(delegate { AddDetect(rigid); });
        }
    }

    void AddDetect(Rigidbody _rigid)
    {
        if (_rigid.velocity.magnitude > 0.1f)
            audioBrain.StartTracking(_rigid);
    }
    
}
