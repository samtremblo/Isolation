using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain_MapAwareness : MonoBehaviour
{
    private Vector3[] wayPoints;
    private Vector3 currentWayPoint;

    private Transform player;


    /*********************************************************/

    private void Awake()
    {
        player = GameObject.FindWithTag("MainCamera").transform;
        Transform[] buffhall = GameObject.FindWithTag("Agent_Waypoints").transform.GetChild(0).GetComponentsInChildren<Transform>();
        Transform[] buffRooms= GameObject.FindWithTag("Agent_Waypoints").transform.GetChild(1).GetComponentsInChildren<Transform>();
        wayPoints = new Vector3[buffhall.Length+buffRooms.Length];
        for (int i = 0; i < buffhall.Length; i++) wayPoints[i] = buffhall[i].position;
        for (int i = 0; i < buffRooms.Length; i++) wayPoints[buffhall.Length + i] = buffRooms[i].position;


    }

    /*********************************************************/

    public Vector3 ChangeWayPoint(int _agressiveLevel)
    {
        List<Vector3> closebuff = new List<Vector3>();
        foreach (Vector3 v in wayPoints)
            if (Vector3.Distance(player.position, v) < (30 - (_agressiveLevel * 5)))
                closebuff.Add(v);

        if (closebuff.Count > 0) currentWayPoint = closebuff[Mathf.FloorToInt(Random.value * closebuff.Count)];
        else currentWayPoint = player.position;
        return currentWayPoint;
    }

    /*********************************************************/

    
}
