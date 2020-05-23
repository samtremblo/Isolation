using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem.Sample
{
    public class trackObj : MonoBehaviour
    {
        private Rigidbody door;
        private GrabTypes grabtype;
        private Coroutine dooring;
        private Quaternion[] limitQuaternion = new Quaternion[2];
        private float limits = 0;
        private bool addVel = false;

        [SerializeField]private bool negative;
        [SerializeField] private float torqMult;

        private void Awake()
        {
            door = GetComponent<Rigidbody>();
            door.maxAngularVelocity = 200;
            limitQuaternion[0] = Quaternion.Euler(0, door.transform.localRotation.y + GetComponent<HingeJoint>().limits.min, 0);
            limitQuaternion[1] = Quaternion.Euler(0, door.transform.localRotation.y + GetComponent<HingeJoint>().limits.max, 0);
            limits = Mathf.Abs(GetComponent<HingeJoint>().limits.max - GetComponent<HingeJoint>().limits.min);

        }
        /*
        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(!addVel)
                {
                    addVel = true;
                    dooring = StartCoroutine(AttachAndFollow());                   
                }
                else
                {
                    //if(dooring != null)StopCoroutine(dooring);
                    addVel = false;
                }
            }
                
        }*/
        private void HandHoverUpdate(Hand hand)
        {
            GrabTypes startGrab = hand.GetGrabStarting();

            if (startGrab != GrabTypes.None && startGrab != grabtype)
            {
                dooring = StartCoroutine( AttachAndFollow(hand.transform) );
            }
            else if ( dooring != null)
                StopCoroutine(dooring);

            grabtype = startGrab;
        }

        private IEnumerator AttachAndFollow(Transform hand)
        {
            Vector3 previousRot= Vector3.zero;
            float rotDelta = 0;
            while (addVel)
            {              
                Vector3 look =( hand.position - door.position );
                if (negative)
                {
                    look = -look;
                }
                Quaternion finalRot = Quaternion.Euler(0, Quaternion.LookRotation(look).eulerAngles.y,0);

                if (Quaternion.Angle(finalRot, limitQuaternion[0]) > 0 &&  Quaternion.Angle(finalRot, limitQuaternion[1]) > 0)
                    if(Quaternion.Angle(finalRot, limitQuaternion[0]) < limits && Quaternion.Angle(finalRot, limitQuaternion[1]) < limits)
                        door.MoveRotation( finalRot );
                rotDelta = door.transform.rotation.eulerAngles.y -previousRot.y ;
                previousRot = door.transform.rotation.eulerAngles;
                yield return null;
            }
            
            door.AddTorque(0,rotDelta* torqMult, 0);
            StopCoroutine(dooring);
        }
    }
}