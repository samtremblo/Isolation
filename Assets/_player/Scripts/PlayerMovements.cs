using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.InteractionSystem.Sample;

public class PlayerMovements : MonoBehaviour
{
    [SerializeField] private float yVelTrigger;
    [SerializeField] private float handsVelTrigger;
    [SerializeField] private float playerInitialSpeed;
    [SerializeField] private float playerRunningSpeed;
    [SerializeField] private float playerAcceleration;
    [SerializeField] private float walkdist;
    [HideInInspector] public bool IsInWall = false;
    public SteamVR_Action_Boolean grapgrip = SteamVR_Input.GetBooleanAction("GrapGrip");


    private float runningLerp;
    private float runningTimer;

    private Transform vrCam;
    private Transform[] hands = new Transform[2];
    private Player player;
    

    private float lastHeadPos;
    private float[] lastHandsPos = new float[2];
    private int lastSign=0;

    private Coroutine walks;

    private Text[] debugs;

    /*****************************************
     ** **/
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        player = GetComponent<Player>();
        hands[0] = transform.GetChild(0).GetChild(1).transform;
        hands[1] = transform.GetChild(0).GetChild(2).transform;
        if (GameObject.Find("VRCamera")) vrCam = GameObject.Find("VRCamera").transform;
        else vrCam = GameObject.Find("FallbackObjects").transform;
    }


    private void Start()
    {
        
        //debugs= GameObject.Find("debug1").GetComponentsInChildren<Text>();
        
        //StartCoroutine(Calibration());


    }


    /*****************************************
     ** updates **/

    private void FixedUpdate()
    {

        //if (!calibrated) return;
        if( CheckForWalk2() && !IsInWall)
        {
            runningTimer = Time.time;
            Vector3 directionWalk = vrCam.TransformDirection(new Vector3(0, 0, walkdist));
            Vector3 dir = player.bodyDirectionGuess* runningLerp;
            //Vector3 newMovement = new Vector3(transform.position.x+directionWalk.x, transform.position.y, transform.position.z+directionWalk.z);
            Vector3 newMovement = new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z + dir.z);

            if (walks != null) StopCoroutine(walks);
            walks = StartCoroutine(walkPlayer(newMovement));
        }

        if(Time.time - runningTimer > 1)
        {
            runningLerp = 0;
            if(walks != null) StopCoroutine(walks);
        }
    }

    void Update()
    {

        SkeletonUIOptions.AnimateHandWithoutController();
        SkeletonUIOptions.HideController();
    }

    /*****************************************/




    /*****************************************
     ** movement tools **/ 
    
    private IEnumerator walkPlayer(Vector3 movement)
    {
        while (Vector3.Distance(transform.position, movement) > 0.1f)
        {
            float speed = Mathf.Lerp(playerInitialSpeed, playerRunningSpeed, runningLerp);
           
            transform.position = Vector3.Lerp(transform.position, movement, speed * Time.deltaTime);

            yield return null;
        }
        yield return null;
    }

    /*****************************************/



    /*****************************************
     ** input tools **/
    #region yes or no
        /*
    private IEnumerator WaitForYes(CoroutineReturn results)
    {
        float lastVelocity = 0;
        float lastRotation = 0;
        int directionShiftCount = 0;
        float resetTimer = Time.time;

        while (directionShiftCount < 4)
        {
            if(player.isHeadsetOn)
            {
                float vel = (vrCam.rotation.x - lastRotation) / Time.deltaTime;

                if (Mathf.Sign(vel) != Mathf.Sign(lastVelocity) && Mathf.Abs(vel) >= yesNoVelocity)
                {
                    directionShiftCount++;
                    resetTimer = Time.time;
                }

                if (directionShiftCount > 0 && Time.time - resetTimer >= 0.5f) directionShiftCount = 0;
                lastVelocity = vel;
                lastRotation = vrCam.rotation.x;       
            }
            yield return null;
        }
        results.boolResult = true;
        yield return null;
    }

    private IEnumerator WaitForNo(CoroutineReturn results)
    {
        float lastVelocity = 0;
        float lastRotation = 0;
        int directionShiftCount = 0;
        float resetTimer = Time.time;

        while (directionShiftCount < 4)
        {
            if (player.isHeadsetOn)
            {
                float vel = (vrCam.rotation.y - lastRotation) / Time.deltaTime;

                if (Mathf.Sign(vel) != Mathf.Sign(lastVelocity) && Mathf.Abs(vel) >= yesNoVelocity)
                {
                    directionShiftCount++;
                    resetTimer = Time.time;
                }

                if (directionShiftCount > 0 && Time.time - resetTimer >= 0.5f) directionShiftCount = 0;
                lastVelocity = vel;
                lastRotation = vrCam.rotation.y;
            }
            yield return null;
        }
        results.boolResult = true;
        yield return null;
    }
    */
    #endregion

    #region stance
    private bool Standing()
    {
       // if (vrCam.position.y >= playerHeightStanding - ((playerHeightStanding - playerHeightCrouch) * 0.25f))
            return true;
        //else return false;
    }
    private bool Crouching()
    {
       // if (vrCam.position.y <= playerHeightCrouch + ((playerHeightStanding - playerHeightCrouch) * 0.5f)  && vrCam.position.y >= playerHeightCrouch * 0.5f )
            return true;
        //else return false;
    }
    #endregion

    private bool CheckForWalk2()
    {
        
        if (!grapgrip.GetState(SteamVR_Input_Sources.LeftHand) || !grapgrip.GetState(SteamVR_Input_Sources.RightHand))
            return false;

        bool trigged = false;

        float[] handVels = new float[] { (hands[0].position.y - lastHandsPos[0]) / Time.deltaTime, (hands[1].position.y - lastHandsPos[1]) / Time.deltaTime };

        if (Mathf.Sign(handVels[0]) != Mathf.Sign(handVels[1]))
        {
            if (Mathf.Abs(handVels[0]) >= handsVelTrigger && Mathf.Abs(handVels[1]) >= handsVelTrigger)
            {
                if(lastSign != Mathf.Sign(handVels[0]) )
                {
                    if (runningLerp < 1) runningLerp += playerAcceleration;
                    if (runningLerp > 1) runningLerp = 1;
                    lastSign = Mathf.FloorToInt(Mathf.Sign(handVels[0]));
                }
                trigged = true;
            }              
            else
                trigged = false;
        }
        else
            trigged = false;

        lastHandsPos[0] = hands[0].position.y;
        lastHandsPos[1] = hands[1].position.y;
        
        return trigged;
    }

    private bool CheckForWalk()
    {
        bool[] trigged = new bool[2] { false, false };

        float headVel = (vrCam.position.y - lastHeadPos) / Time.deltaTime;

        float[] handVels = new float[] { (hands[0].position.y - lastHandsPos[0]) / Time.deltaTime, (hands[1].position.y - lastHandsPos[1]) / Time.deltaTime };

        if (Mathf.Sign(handVels[0]) != Mathf.Sign(handVels[1]))
        {
            if (Mathf.Abs(handVels[0]) >= handsVelTrigger && Mathf.Abs(handVels[1]) >= handsVelTrigger)
                trigged[1] = true;
            else
                trigged[1] = false;
        }
        else
            trigged[1] = false;

        if (headVel >= yVelTrigger)
            trigged[0] = true;
        else
            trigged[0] = false;

        lastHandsPos[0] = hands[0].position.y;
        lastHandsPos[1] = hands[1].position.y;
        lastHeadPos = vrCam.position.y;

        if (trigged[0] && trigged[1])
            return true;
        else
            return false;
        
    }
    /*****************************************/



    /*****************************************
     ** just tools **/

    private class CoroutineReturn
    {
        public bool boolResult= false;
        public float floatResult= 0;
        public Vector2 vec2Result= Vector2.zero;
        public Vector3 vec3Result= Vector3.zero;
        public int intResult = 0;

        public void reset()
        {
            boolResult = false;
            floatResult = 0;
            vec2Result = Vector2.zero;
            vec3Result = Vector3.zero;
            intResult = 0;
        }
    }

    /*****************************************
     ** start calibration **/
     /*
    private IEnumerator Calibration()
    {
        int steps = 0;
        bool headJustPlaced = false; 
        Coroutine waitHeadInput = null;

        while (!calibrated)
        {
            if(steps==0)
            {
                if (player.isHeadsetOn)
                {
                    if (!headJustPlaced)
                    {
                        yield return new WaitForSeconds(3);
                        headJustPlaced = true;
                        //debug1.text = "Are you standing up?";
                    }
                    if (waitHeadInput == null) waitHeadInput = StartCoroutine(WaitForYes(results));
                    if (results.boolResult)
                    {
                        //playerHeightStanding = vrCam.position.y;
                        steps = 1;
                        StopCoroutine(waitHeadInput);
                        results.reset();
                        waitHeadInput = null;
                        calibrated = true;
                    }
                }
            }
            else if(steps == 1)
            {          
                if (player.isHeadsetOn)
                {
                    if (waitHeadInput == null) waitHeadInput = StartCoroutine(WaitForYes(results));
                    if (results.boolResult)
                    {
                        //playerHeightCrouch = vrCam.position.y;
                        StopCoroutine(waitHeadInput);
                        results.reset();
                        calibrated = true;
                    }
                }
                //debug1.text = "Are you crouching?";
                //debug2.text = "standing height: " + playerHeightStanding.ToString();
            }
            yield return null;
            
        }
        //debug1.text = "";
        yield return null;
    }
    */
}
