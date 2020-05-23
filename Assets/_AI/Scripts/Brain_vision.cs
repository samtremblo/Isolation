using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain_vision : MonoBehaviour
{
    private Coroutine visionAnalysis;
    private Transform headAgent;

    [HideInInspector] public bool iCanSeeYou= false;



    private void Awake()
    {
        headAgent = GetComponentInParent<Transform>();
    }


    /*********************************************************/

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "HeadCollider") visionAnalysis = StartCoroutine(VisionAnalysis(other.transform));
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "HeadCollider")
        {
            transform.rotation = Quaternion.identity;
            if(visionAnalysis != null)StopCoroutine(visionAnalysis);
        }
    }

    /*********************************************************/

    private IEnumerator VisionAnalysis(Transform player)
    {
        bool lostContact = false;
        while (!lostContact)
        {
            Vector3 dir = player.position - headAgent.position;
            transform.rotation = Quaternion.FromToRotation(transform.forward, dir);

            if (Physics.Raycast(headAgent.position, dir, out RaycastHit hit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, dir * hit.distance, Color.yellow);
                if (hit.collider.name == "HeadCollider")
                {
                    iCanSeeYou = true;
                }
                else
                {
                    iCanSeeYou = false;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            }
            yield return null;
        }
        yield return null;
    }
}
