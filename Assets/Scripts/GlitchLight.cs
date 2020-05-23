using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchLight : MonoBehaviour
{
    Light theLight;
    public float StartStrenght;

    // Start is called before the first frame update
    void Start()
    {
        theLight = GetComponent<Light>();
        theLight.intensity = StartStrenght;

    }

    // Update is called once per frame
    void Update()
    {
        theLight.intensity = theLight.intensity + Random.Range(-.1f, .1f);

        if (theLight.intensity > .5f + StartStrenght)
        {
            theLight.intensity = .5f + StartStrenght;
        }

        if (theLight.intensity < .1f)
        {
            theLight.intensity = .1f;
        }




    }
}
