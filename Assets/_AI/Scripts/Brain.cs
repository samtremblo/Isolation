using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    private Brain_vision vision;
    private Brain_MapAwareness mapAwareness;
    private Brain_Behavior behaviors;
    private Brain_Audition audition;
    private DisolveSphere disolver;
    private PlayerFuckery playerFuckery;
    private Transform head;

    private int agressiveLevel = 0;
    public int AgressiveLevel
    {
        get { return agressiveLevel; }
        set { agressiveLevel = value; }
    }

    private void Awake()
    {
        mapAwareness = gameObject.AddComponent<Brain_MapAwareness>();
        playerFuckery = gameObject.AddComponent<PlayerFuckery>();
        behaviors = gameObject.AddComponent<Brain_Behavior>();
        vision = FindObjectOfType<Brain_vision>().GetComponent<Brain_vision>();

        audition = FindObjectOfType<Brain_Audition>().GetComponent<Brain_Audition>();
        audition.brain = this;

        head = transform.parent;
        disolver = FindObjectOfType<DisolveSphere>();
    }

    private void Update()
    {
        if (behaviors.CurrentBehavior == Brain_Behavior.Behaviors.Idle)
        {
            behaviors.Roaming(mapAwareness.ChangeWayPoint(agressiveLevel));
        }


        if (vision.iCanSeeYou)
        {
            behaviors.ChasePlayer(true);
        }
        else if (behaviors.CurrentBehavior == Brain_Behavior.Behaviors.Chasing) behaviors.ChasePlayer(false);


        //if need behavior
  
        //movements

        //shaderModif

        //player control fucker (communication with scripts)
        ////control fucker cases

        //animation handling

        // some voodo magic i guess
    }

    public void OnHeardSomething(int _intense)
    {
        if (_intense == 1) behaviors.Roaming(audition.objsHeard);
        if (_intense == 2) behaviors.AgressiveRoaming(audition.objsHeard);
        audition.objsHeard = Vector3.zero;
    }
}
