using UnityEngine;

public class P_MainStorm : MonoBehaviour
{
    /****
    **********/
    #region IDs
    private int headPosID;
    private int deltaID;
    private int attractorID;
    private int heartbeatID;
    #endregion
    /****
    **********/
    #region public settings
    public float bpm;
    public Vector2 MinMaxPI_Divisor;
    private float heartSpeed;
    private float oldbpm;
    private float beatTimer
    {
        get
        {
            return 60/oldbpm;
        }
        set
        {
            oldbpm = value;
            heartSpeed = Mathf.Lerp(Mathf.PI / MinMaxPI_Divisor.x, Mathf.PI / MinMaxPI_Divisor.y, Mathf.Clamp(1 - bpm / 60, 0, 1));
        }
    }

    #endregion

    /****
     **********/
    #region particles
    private struct Particles
    {
        public float colorLife;
        public Vector3 position;
        public Vector3 localPos;
        public float life;
    }
    Particles[] particleBuffer;

    private Vector3 attractor = new Vector3(0, 0, 0);
    private float attractorClock = 0;
    private float[] heartBeats = new float[2];
    private float bpmClock = 0;
    private const float HALF_PI = Mathf.PI / 2;
    private const float TWO_PI = 2 * Mathf.PI;

    #endregion

    /****
    ***********/
    #region Externals
    private ShaderInputs inputs;

    private Material particleMat;
    private int particleCount;
    [HideInInspector] public ComputeShader computeShader;
    private bool launching= true;
    private bool ready = false;
    private Transform headPosition;  
    #endregion

    /****
    ***********/
    #region compute data
    private const int WARP_SIZE = 512;
    private const int PS_DATA_SIZE = 32;
    private int warpCount;
    private int ComputeKernelID;

    private ComputeBuffer computeBuffer;
    #endregion


    /**************************************************************************/

    #region startup
    private void Awake()
    {
        beatTimer = bpm;
        headPosition = GameObject.FindWithTag("Agent_Head").transform;
        inputs = GetComponent<ShaderInputs>();
        particleMat = inputs.particleMaterial;
        particleCount = inputs.particleCounts;
        computeShader = inputs.computeShader;
    }

    void Start()
    {
        headPosID= Shader.PropertyToID("headPosition");
        deltaID = Shader.PropertyToID("deltaTime");
        attractorID = Shader.PropertyToID("attractor");
        heartbeatID = Shader.PropertyToID("heartBeats");

        warpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);
        ComputeKernelID = computeShader.FindKernel("CSMainStorm");

        particleBuffer = StartupParticles(particleCount);

        computeBuffer = new ComputeBuffer(particleCount, PS_DATA_SIZE);
        computeBuffer.SetData(particleBuffer);

        computeShader.SetBuffer(ComputeKernelID, "particleBuffer", computeBuffer);
        particleMat.SetBuffer("particleBuffer", computeBuffer);  

    }
    #endregion

    /**************************************************************************/

    public void Spawn(bool _intialisation)
    {
        if (_intialisation)
        {
            computeShader.SetFloat("speed", 0.1f);
            ready = true;     
        }
            
        else
        {
            launching = false;
            computeShader.SetFloat("speed", 1f);
        }
    }

    public void Despawn()
    {
        ready = false;
        computeBuffer.SetData(particleBuffer); //clean reboot
        computeShader.Dispatch(ComputeKernelID, warpCount, 1, 1); //apply clean
    }

    /**************************************************************************/

    void Update()
    {
        if(!launching)
        {
            UpDateAttractor();
            HeartBeat();
            Dispatching();
        }
        else if (ready) Dispatching();
    }

    private void HeartBeat()
    {
        if (Time.time - bpmClock > beatTimer)
        {
            if (heartBeats[0] > HALF_PI) heartBeats[1] += heartSpeed;
            if (heartBeats[0] < TWO_PI) heartBeats[0] += heartSpeed;
            else
            {
                bpmClock = Time.time;
                heartBeats[0] = 0;
                heartBeats[1] = 0;
            }

        }
        
    }

    private void UpDateAttractor()
    {
        attractorClock += Mathf.PI / 512;

        if (attractorClock > 3 * Mathf.PI) attractorClock = 0;
        else
        {
            attractor.x = Mathf.Sin(attractorClock);
            attractor.z = Mathf.Cos(attractorClock);
            attractor.y = Mathf.Sin(attractorClock);
        }
        
        attractor += headPosition.position;
    }


    private void Dispatching()
    {
        computeShader.SetVector(headPosID, headPosition.position);
        computeShader.SetFloat(deltaID, Time.deltaTime);
        computeShader.SetVector(attractorID, attractor);
        computeShader.SetFloats(heartbeatID, heartBeats);
        computeShader.Dispatch(ComputeKernelID, warpCount, 1,1);
    }


    /**************************************************************************/

    private Particles[] StartupParticles(int count)
    {
        Particles[] buff = new Particles[count];
        
        for(int i=0; i< buff.Length; i++)
        {
            float x = Random.value * 2 - 1.0f;
            float y = Random.value * 2 - 1.0f;
            float z = Random.value * 2 - 1.0f;
            Vector3 xyz = new Vector3(x, y, z);
            xyz.Normalize();
            xyz *= Random.value;
            xyz *= 0.5f;


            buff[i].localPos.x = headPosition.position.x+xyz.x;
            buff[i].localPos.y = headPosition.position.y + xyz.y;
            buff[i].localPos.z = headPosition.position.z + xyz.z;

            buff[i].position = headPosition.position;

            buff[i].life = Random.value * 5.0f + 1.0f;
            buff[i].colorLife = 0;
        }

        return buff;
    }


    void OnDestroy()
    {
        if (computeBuffer != null)
            computeBuffer.Release();
    }
}
