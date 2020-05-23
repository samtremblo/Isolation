using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicRestrictions : MonoBehaviour
{
    private float maxOneHanded = 1.0f;
    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // handhoverupdate()
}
