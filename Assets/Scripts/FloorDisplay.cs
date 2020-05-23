using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Add the TextMesh Pro namespace to access the various functions.

public class FloorDisplay : MonoBehaviour
{
    public TextMeshPro m_Text;
    public GameObject theDoors;
    public GameObject theElevator;

    private int currentValue = 5;
    public float TimePerLevels = 1;
    private float timerDelay, TimeReset;
    string[] levels = { "B2", "B1", "0","1","2","3"};    // 0-5
    void Start()
    {
        theDoors = GameObject.Find("elevator_door_left");
        theElevator = GameObject.Find("elevator_real");

        m_Text =   gameObject.GetComponent<TextMeshPro>();

        StartCoroutine(FloorChanging());
        m_Text.text = levels[currentValue];

    }

    // Update is called once per frame
    IEnumerator FloorChanging()

    {
        StartCoroutine(theElevator.GetComponent<ElevatorWiggle>().GoingUpCoroutine());

        TimeReset = Time.time;

        while (currentValue>-1) {

            if (Time.time - TimeReset > TimePerLevels)
            {
                m_Text.text = levels[currentValue];
                currentValue--;
                TimeReset = Time.time;
            }
            yield return null;

        }

        StartCoroutine(theElevator.GetComponent<ElevatorWiggle>().ArrivedCoroutine());
        yield return new WaitForSeconds(3);

        StartCoroutine(theDoors.GetComponent<ElevatorDoors>().OpenDoors());
      

    }
}
