using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorWiggle : MonoBehaviour
{
    public static bool isGoingUp = true;
    public static bool isArriving = false;
    public  bool hasArrived = false;

    private float wiggle,t,t2,range,rangeElevator = 0.009f;

    public IEnumerator GoingUpCoroutine()
    {
     
        while(!isArriving)
        {
            t = Mathf.Abs(-Mathf.Sin(Time.time * 8));
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, .12f + t * rangeElevator, gameObject.transform.position.z);
            yield return null;
         }

        }


 public IEnumerator ArrivedCoroutine()
    {
        isArriving = true;

        while (isArriving)
        {
            rangeElevator = Mathf.Lerp(0.04f, 0, t2);
            t2 += Time.deltaTime;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, .12f + t * rangeElevator, gameObject.transform.position.z);

            yield return null;
        }

    }

     



    }




