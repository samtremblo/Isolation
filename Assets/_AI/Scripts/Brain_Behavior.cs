using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Brain_Behavior : MonoBehaviour
{
    public enum Behaviors
    {
        Idle,
        Chasing,
        Roaming,
        RoamingAgressive,
        AudioTracking,
        Waiting,
        TouchingPlayer
    }
    private Behaviors currentBehavior;
    public Behaviors CurrentBehavior
    {
        get { return currentBehavior; }
        protected set { currentBehavior = value; }
    }

    private NavMeshAgent agent;
    private Transform player;
    private Coroutine chasingPlayer;
    private Coroutine moveprogress;
    private Coroutine beLazy;

    private void Awake()
    {
        agent = GameObject.FindWithTag("Agent_Head").GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("MainCamera").transform;
    }


    /*********************************************************/

    public void Roaming(Vector3 _destination)
    {
        agent.acceleration = 0.5f;
        agent.angularSpeed = 80;
        agent.speed = 1;
        CurrentBehavior =  agent.SetDestination(_destination) ? Behaviors.Roaming : Behaviors.Idle ;
        if (CurrentBehavior == Behaviors.Roaming && moveprogress == null)
            moveprogress = StartCoroutine(Moveprogress());
    }

    public void AgressiveRoaming(Vector3 _destination)
    {
        agent.acceleration = 0.5f;
        agent.angularSpeed = 100;
        agent.speed = 3;
        CurrentBehavior = agent.SetDestination(_destination) ? Behaviors.RoamingAgressive : Behaviors.Idle;
        if (CurrentBehavior == Behaviors.RoamingAgressive && moveprogress == null)
            moveprogress = StartCoroutine(Moveprogress());
    }

    public void ChasePlayer(bool _start)
    {
        CurrentBehavior = _start ? Behaviors.Chasing : Behaviors.RoamingAgressive;
        if (CurrentBehavior == Behaviors.Chasing && chasingPlayer == null)
            chasingPlayer = StartCoroutine(ChasingPlayer());
        else 
            if (chasingPlayer != null) StopCoroutine(chasingPlayer);
    }

    public void TrackAudio(Vector3 _destination)
    {
        agent.acceleration = 0.5f;
        agent.angularSpeed = 80;
        agent.speed = 1;
        CurrentBehavior = agent.SetDestination(_destination) ? Behaviors.AudioTracking : Behaviors.Idle;
        if (CurrentBehavior == Behaviors.AudioTracking && moveprogress == null)
            moveprogress = StartCoroutine(Moveprogress());
    }
/*********************************************************/

private IEnumerator ChasingPlayer()
    {
        bool ischasing = true;
        moveprogress = StartCoroutine(Moveprogress());
        while (ischasing && CurrentBehavior == Behaviors.Chasing)
        {
            ischasing = agent.SetDestination(player.position);
            yield return null;
        }
        
        yield return null;
    }

    private IEnumerator Moveprogress()
    {
        while(CurrentBehavior != Behaviors.Idle && currentBehavior != Behaviors.Waiting && currentBehavior != Behaviors.TouchingPlayer)
        {
            if (agent.remainingDistance < agent.stoppingDistance && !agent.hasPath)
            {
                CurrentBehavior = CurrentBehavior == Behaviors.Chasing ? Behaviors.TouchingPlayer : Behaviors.Waiting;
                if (CurrentBehavior == Behaviors.Waiting)
                    beLazy = StartCoroutine(WaitSomeBit(Random.value * 5 + 4));
            }
            yield return null;
        }
        yield return null;
    }

    private IEnumerator WaitSomeBit(float time)
    {
        yield return new WaitForSeconds(time);
        CurrentBehavior = Behaviors.Idle;
        yield return null;
    }
}
