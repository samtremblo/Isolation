using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform Target;
    public float OffsetX, OffsetY, OffsetZ;
    public float maxRotationX, maxRotationY;

    // Start is called before the first frame update
    void Start()
    {
      Target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
       
        Vector3 direction = Target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(-direction +  new Vector3(OffsetX, OffsetY, OffsetZ));
       

        transform.rotation = rotation;

     //   Vector3 currentRotation = transform.localRotation.eulerAngles;
     //   currentRotation.y = Mathf.Clamp(currentRotation.y, -maxRotationY, maxRotationY);
     //   transform.localRotation = Quaternion.Euler(currentRotation);
    }
}
