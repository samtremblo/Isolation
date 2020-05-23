using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain_Audition : MonoBehaviour
{
    [SerializeField] private float minInterestDistance;
    [SerializeField] private float minAngerDistance;
    [HideInInspector] public int HeardSomething;
    [HideInInspector] public Brain brain;

    private Coroutine[] trackingAudio = new Coroutine[6];
    public Vector3 objsHeard;

    private void Awake()
    {
        AudioRelays[] relays = FindObjectsOfType<AudioRelays>();
        foreach (AudioRelays relay in relays) relay.brainAudioProcess = this;
    }
    /*********************************************************/

    public void StartTracking(Rigidbody _object)
    {
        for (int i = 0; i< trackingAudio.Length ; i++)
        {
            if (trackingAudio[i] == null)
            {
                trackingAudio[i] = StartCoroutine( TrackingAudio(_object,i) );
                break;
            }
        }
    }

    private void CheckClosest(Vector3 _pos, int _intense)
    {
        bool changed = false;
        if (objsHeard == Vector3.zero || Vector3.Distance(transform.position, _pos) < Vector3.Distance(transform.position, objsHeard))
        {
            objsHeard = _pos;
            changed = true;
        }
        if (changed) brain.OnHeardSomething(_intense);
    }
    /*********************************************************/


    private IEnumerator TrackingAudio(Rigidbody _obj, int index)
    {
        float vel = _obj.velocity.magnitude;
        float soundVolume = vel;
        print("initil vel: " + vel);

        while(Mathf.Abs(vel) > 0.1f)
        {
            if (Mathf.Abs(vel) > soundVolume) soundVolume = vel;
            vel = _obj.velocity.magnitude;
            Debug.DrawRay(transform.position, _obj.position- transform.position, Color.yellow);
            yield return null;
        }

        float dist = Vector3.Distance(transform.position, _obj.position);
        print("dist: "+dist);

        if (dist < minAngerDistance)
        {
            HeardSomething = 2;
            CheckClosest(_obj.position, 2);
            print("i can hear youuu");
        }
        else if (dist < minInterestDistance)
        {
            HeardSomething = 1;
            CheckClosest(_obj.position, 1);
            print("big think");
        }        
        StopCoroutine(trackingAudio[index]);
        yield return null;
    }
  
}
