using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Valve.VR.InteractionSystem.Sample
{
    public class DoorTrackHand : MonoBehaviour
    {
        private Rigidbody door;
        private GrabTypes grabtype;
        private Coroutine dooring;
        private Quaternion[] limitQuaternion = new Quaternion[2];
        private float limits = 0;

        [SerializeField] private UnityEvent onPickup;
        [SerializeField] private UnityEvent onRelease;
        [SerializeField] private bool negative;
        [SerializeField] private float torqMult;
        [SerializeField] private float rotOffset;
        [SerializeField] private bool oneParent;

        private void Awake()
        {
            door = GetComponent<Rigidbody>();
            door.maxAngularVelocity = 200;
            float angleOffset;
            if(!oneParent) angleOffset= rotOffset - transform.parent.parent.transform.eulerAngles.y;
            else angleOffset= rotOffset - transform.parent.transform.eulerAngles.y;
            limitQuaternion[0] = Quaternion.Euler(0, door.transform.localRotation.y + GetComponent<HingeJoint>().limits.min - angleOffset, 0);
            limitQuaternion[1] = Quaternion.Euler(0, door.transform.localRotation.y + GetComponent<HingeJoint>().limits.max - angleOffset, 0);
            limits = Mathf.Abs(GetComponent<HingeJoint>().limits.max - GetComponent<HingeJoint>().limits.min);

        }

        private void HandHoverUpdate(Hand hand)
        {
            GrabTypes startGrab = hand.GetGrabStarting();
            if (startGrab != GrabTypes.None && startGrab != grabtype)
            {
                grabtype = startGrab;
                onPickup.Invoke();
                if (dooring != null) StopCoroutine(dooring);
                dooring = StartCoroutine( AttachAndFollow(hand) );
            }
        }

        private IEnumerator AttachAndFollow(Hand _hand)
        {
            Transform hand = _hand.transform;
            Vector3 previousRot= Vector3.zero;
            float rotDelta = 0;
            while (_hand.IsGrabbingWithType(grabtype))
            {
                
                Vector3 look =( hand.position - door.position );
                
                if (negative)
                {
                    look = -look;
                }
                
                Quaternion finalRot = Quaternion.Euler(0, Quaternion.LookRotation(look).eulerAngles.y,0);
                if (Quaternion.Angle(finalRot, limitQuaternion[0]) > 0 && Quaternion.Angle(finalRot, limitQuaternion[1]) > 0)
                    if (Quaternion.Angle(finalRot, limitQuaternion[0]) < limits && Quaternion.Angle(finalRot, limitQuaternion[1]) < limits)
                    {
                        door.MoveRotation(finalRot);
                    }
                rotDelta = door.transform.rotation.eulerAngles.y -previousRot.y ;
                previousRot = door.transform.rotation.eulerAngles;
                yield return null;
            }
            onRelease.Invoke();
            grabtype = GrabTypes.None;
            door.AddTorque(0,(rotDelta/Time.deltaTime )* torqMult, 0);
        }
    }
}