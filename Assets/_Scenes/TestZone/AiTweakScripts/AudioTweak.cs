using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTweak : MonoBehaviour
{
    private Brain_Audition audition;
    private Brain_Behavior behaviors;
    private Transform head;

    private void Awake()
    {
        audition = FindObjectOfType<Brain_Audition>().GetComponent<Brain_Audition>();
        //audition.brain = this;
        behaviors = gameObject.AddComponent<Brain_Behavior>();
        head = GameObject.FindWithTag("Agent_Head").transform;
    }

    private void Update()
    {
    }

    public void OnHeardSomething(int _intense)
    {
        if (_intense == 1) behaviors.Roaming(audition.objsHeard);
        if (_intense == 2) behaviors.AgressiveRoaming(audition.objsHeard);
        audition.objsHeard = Vector3.zero;
    }
}
