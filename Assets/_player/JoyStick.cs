using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class JoyStick : MonoBehaviour
{
    public float PlayerSpeed;
    private SteamVR_Action_Vector2 moveAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("Player", "Walk");
    public SteamVR_Input_Sources hand;
    public SteamVR_ActionSet activateAction;
    private Transform head;
    

    void Start()
    {
        activateAction.Activate(hand);
        head = GameObject.Find("VRCamera").transform;
    }

    void Update()
    {
        //transform.rotation = head.rotation;
        hand = SteamVR_Input_Sources.LeftHand;
        Vector2 m = moveAction[hand].axis;
        m *= PlayerSpeed/10;
        Vector3 addMove= new Vector3(m.x, 0, m.y);
        addMove = head.transform.TransformDirection(addMove);
        Vector3 movement = new Vector3(addMove.x, 0,addMove.z);
        transform.position += movement;
        //movement = new Vector3(m.x, 0, m.y);
        print(m);
    }
}
