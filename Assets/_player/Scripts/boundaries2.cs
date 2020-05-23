using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boundaries2 : MonoBehaviour
{
    private Transform player;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "hallwayCol")
        {
            Vector3 newPos= transform.forward * 0.05f;
            newPos.y = 0;
            player.position -= newPos;
        }
    }
}
