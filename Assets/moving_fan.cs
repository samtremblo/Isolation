using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving_fan : MonoBehaviour
{

    public bool fanIsMoving = true;
    public float speedMovingFan = 500f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fanIsMoving == true) {
            moveFan();
        }
    }

    private void moveFan() {

        transform.Rotate(0f, speedMovingFan * Time.deltaTime, 0f);
    }
}
