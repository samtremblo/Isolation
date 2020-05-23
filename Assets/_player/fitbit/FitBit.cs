using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitBit : MonoBehaviour
{
    public float RadarSensivity = .01f;

    public Transform target;
    private Transform player;
    public bool activated = false;
    private Material radarMat;

    private int agentDistID;
    private int playerPosID;
    private int agentRotationID;
    private int playerRotationID;

    private struct ColorMasks
    {
        public int colorMaskID;
        public Vector3 red;
        public Vector3 blue;
        public Vector3 pink;
        public Vector3 yellow;
        public Vector3 cyan;
        public Vector3 green;
    }

    ColorMasks colorMasks;


    private void Start()
    {
        radarMat = GameObject.FindWithTag("radar").GetComponent<Renderer>().material;
        target = GameObject.FindWithTag("Agent_Head").transform;
        player = GameObject.FindWithTag("MainCamera").transform;
        print(player.name);

        GameObject.FindWithTag("Player_Head_collider").AddComponent<FitBit_head>().fitBit= this;

        colorMasks = new ColorMasks();
        agentDistID = Shader.PropertyToID("_AgentDist");
        playerPosID = Shader.PropertyToID("_PlayerDist");
        agentRotationID = Shader.PropertyToID("_AgentRotation");
        playerRotationID = Shader.PropertyToID("_PlayerRotation");

        colorMasks.colorMaskID = Shader.PropertyToID("_ColorMask");
        colorMasks.red = new Vector3(1, 0, 0);
        colorMasks.blue= new Vector3(0, 0,1);
        colorMasks.pink= new Vector3(1, 0, 1);
        colorMasks.yellow= new Vector3(1, 1, 0);
        colorMasks.cyan= new Vector3(0, 1, 1);
        colorMasks.green = new Vector3(0, 1, 0);
    }


    public void ChangeColorMask(string name)
    {
        if (name == "room1" || name == "room5") radarMat.SetVector(colorMasks.colorMaskID, colorMasks.red);
        if (name == "room2") radarMat.SetVector(colorMasks.colorMaskID, colorMasks.blue);
        if (name == "room3") radarMat.SetVector(colorMasks.colorMaskID, colorMasks.pink);
        if (name == "room4") radarMat.SetVector(colorMasks.colorMaskID, colorMasks.yellow);
        if (name == "room6") radarMat.SetVector(colorMasks.colorMaskID, colorMasks.cyan);
        if (name == "hall") radarMat.SetVector(colorMasks.colorMaskID, colorMasks.green);
    }

    private void Update()
    {
        //we don't want the height to affect anything, so let's treat them as 2D to get the distance

        Vector2 plannarPlayer = new Vector2(player.position.x, player.position.z);
        Vector2 plannarTarget = new Vector2(target.position.x, target.position.z);

        float radiusDistanceFromPlayer = Vector2.Distance(plannarPlayer, plannarTarget) * RadarSensivity;
        float playerDistance = Vector2.Distance(plannarPlayer, Vector2.zero);

        /************************************************************************************/
        //now we get the angle offset of the  target's direction from the front of the player

        Vector3 directionpPlayerTarget = target.position - player.position;
        directionpPlayerTarget.y = 0;//fuck the height
        Quaternion rotationFromFrontToTarget = Quaternion.FromToRotation(player.forward, directionpPlayerTarget);

        //rotationAroundPlayerOffset = rotationFromFrontToTarget.eulerAngles.y;


        /************************************************************************************/

        radarMat.SetFloat(agentDistID, radiusDistanceFromPlayer);
        radarMat.SetFloat(agentRotationID, rotationFromFrontToTarget.eulerAngles.y);
        radarMat.SetVector(playerPosID, new Vector2(player.position.x, player.position.z));
        radarMat.SetFloat(playerRotationID, player.rotation.eulerAngles.y);
        
    }

   

    void OnDestroy()
    {
        Destroy(radarMat);
    }
}
