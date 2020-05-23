using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoors : MonoBehaviour

{
    private GameObject Door, MirrorDoor;
    private float BougeDoor = .52f;
    private float lastValue;
    private float OffsetY = -0.65f;
    private float t;
    private float speed=.5f;

    public static bool isOpening = false;
    public static bool isClosing = false;
   void Start()
    {
      Door = gameObject;

      MirrorDoor = GameObject.Find("elevator_door_right");

        Door.transform.position = new Vector3(BougeDoor + OffsetY, Door.transform.position.y, Door.transform.position.z);
        MirrorDoor.transform.position = new Vector3(-BougeDoor + OffsetY, MirrorDoor.transform.position.y, MirrorDoor.transform.position.z);

    }

   public  IEnumerator OpenDoors()
    {
       
        while (t<=2)
        {

            speed = Mathf.Lerp(0f, .6f, t / 2);
            BougeDoor = Mathf.Lerp(.52f, 1.61f, t * speed);


            Door.transform.position = new Vector3(BougeDoor + OffsetY, Door.transform.position.y, Door.transform.position.z);
            MirrorDoor.transform.position = new Vector3(-BougeDoor + OffsetY, MirrorDoor.transform.position.y, MirrorDoor.transform.position.z);
            t+= Time.deltaTime;
            yield return null;


        }

        lastValue = BougeDoor;
        t = 0;
        yield return null;

    }



    public IEnumerator CloseDoors()
    {

        while (t<=3)
        {

            BougeDoor = Mathf.Lerp(lastValue, .52f,t/3);


            Door.transform.position = new Vector3(BougeDoor + OffsetY, Door.transform.position.y, Door.transform.position.z);
            MirrorDoor.transform.position = new Vector3(-BougeDoor + OffsetY, MirrorDoor.transform.position.y, MirrorDoor.transform.position.z);
            t += Time.deltaTime;
            yield return null;


        }

        t = 0;
    }





    }

   
