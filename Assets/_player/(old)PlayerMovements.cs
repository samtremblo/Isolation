using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementsOld : MonoBehaviour
{
    public float upDownbobTrigger;
    public float yawnBobTrigger;
    public float headTimetrigger;
    public float playerSpeed;

    private Transform vrCam;
    private Text debug1, debug2;
    private float xAxis, zAxis, xSign, zSign;
    private float headTimer;
    void Start()
    {
        vrCam= GameObject.Find("VRCamera").transform;
        debug1 = vrCam.GetChild(0).GetChild(0).GetComponent<Text>();
        debug2 = vrCam.GetChild(0).GetChild(1).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float velX = (vrCam.rotation.x - xAxis) / Time.deltaTime;
        float velZ = (vrCam.rotation.z - zAxis) / Time.deltaTime;
        xAxis = vrCam.rotation.x;
        zAxis = vrCam.rotation.z;
        if(Time.time- headTimer >= headTimetrigger)
        {
            xSign = Mathf.Sign(velX);
            zSign= Mathf.Sign(velZ);
            debug2.text = Mathf.Abs(velX).ToString();
            headTimer = Time.time;
        }

        if (xSign != Mathf.Sign(velX) && Mathf.Abs(velX) >= upDownbobTrigger)
        {
            xSign = Mathf.Sign(velX);
            zSign = Mathf.Sign(velZ);
            headTimer = Time.time;
            debug2.text = Mathf.Abs(velX).ToString();
            Vector3 moves = vrCam.InverseTransformDirection(new Vector3(0,0,playerSpeed));
            transform.position += new Vector3(0,0,moves.z);
            debug1.text = "yeet";
        }
        else debug1.text = "fuck off";

        


    }
}
