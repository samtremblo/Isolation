using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheckForPlayer : MonoBehaviour

{
    public GameObject theDoors;
    void Start()
    {
        theDoors = GameObject.Find("elevator_door_left");

    }


    void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.name == "Player")
        {
            StopCoroutine(theDoors.GetComponent<ElevatorDoors>().OpenDoors());
            StartCoroutine(theDoors.GetComponent<ElevatorDoors>().CloseDoors());
        }


    }
}
