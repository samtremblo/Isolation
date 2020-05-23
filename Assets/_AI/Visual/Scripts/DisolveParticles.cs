using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class DisolveParticles : MonoBehaviour
{
    //needs a tiny bit of cleaning/optimizing

    #region Shaders IDs
    private int disolvingID;
    private int colorID;

    private int isSolidID;
    private int objColorID;
    private int solidID;
    private int objTexID;
    
    private int disolveLevelID;
    #endregion

    #region externals
    private ShaderInputs inputs;

    private Material particleMat;
    private int particleCount;
    private ComputeShader stormShader;
    #endregion

    #region particles
    private struct ParticlesDisolve
    {
        public Vector3 originVertex;
        public Vector3 position;
        public Vector3 uv;
        public float life;
    }
    private ParticlesDisolve[] disolveBuffer;
    #endregion

    #region compute data
    private const int WARP_SIZE = 512;
    private const int PS_DATA_SIZE = 40;
    private int warpCount;
    private int ComputeKernelID;

    private ComputeBuffer computeBuffer;
    private ComputeBuffer bufferDataBuffer;
    #endregion

    #region objects/instances
    private Transform head;

    private struct ObjectBuffer
    {
        public int offsetInParticles;
        public int offsetInArray;
        public float distSphere;
        public float disolveLevel;
        public float disolveSpeedFactor;
        public Transform transf;
        public Mesh mesh;
        public Coroutine disolve;
        public Coroutine particleRun;
        public Coroutine fadeoff;
    }
    private ObjectBuffer[] objectBuffer;

    private struct BufferData
    {
        public Vector3 objectsPosition;
        public float active;
    }
    private BufferData[] bufferData= new BufferData[10];

    private float[] solidColors = new float[10];
    private Color[] cols=new Color[10];
    private Texture2DArray texArray;

    private Coroutine trackObjects;
    [SerializeField] private int maxObjects;
    #endregion

    /**************************************************************************/

    private void Awake()
    {
        if (maxObjects > 10) maxObjects = 10;
        head = GameObject.FindWithTag("Agent_Head").transform;  

        inputs = GetComponent<ShaderInputs>();
        particleMat = inputs.particleMaterial;
        particleCount = inputs.particleCounts;

        objectBuffer = new ObjectBuffer[maxObjects];
        for (int i = 0; i < objectBuffer.Length; i++)
        {
            objectBuffer[i].offsetInArray = i;
            objectBuffer[i].offsetInParticles = i * (particleCount / maxObjects);
        }

        texArray= new Texture2DArray(1024,1024,maxObjects, TextureFormat.ARGB32, true);
    }

    private void Start()
    {
        stormShader = FindObjectOfType<P_MainStorm>().computeShader;
        GetIDs();

        warpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);
        ComputeKernelID = stormShader.FindKernel("CSDisolve");

        bufferDataBuffer = new ComputeBuffer(bufferData.Length, 16);
        bufferDataBuffer.SetData(bufferData);
        computeBuffer = new ComputeBuffer(particleCount, PS_DATA_SIZE);
        StartCoroutine(RunParticles());

        stormShader.SetBuffer(ComputeKernelID, "disolveBuffer", computeBuffer);
        stormShader.SetBuffer(ComputeKernelID, "bufferData", bufferDataBuffer);
        particleMat.SetBuffer("disolveBuffer", computeBuffer);
        particleMat.SetTexture("_ObjTexArray", texArray);
    }


    /**************************************************************************/

    public void TargetObjectToDestroy(MeshFilter _objectMesh)
    {
        for(int i=0; i< objectBuffer.Length; i++)
        {
            if (objectBuffer[i].disolveLevel <= 0 && bufferData[i].active <=0)
            {
                bufferData[i].active = 1;
                print("target: " + i);
                objectBuffer[i].transf = _objectMesh.transform;
                objectBuffer[i].mesh = _objectMesh.mesh;
                DestroyThisObject(i);       
                if (trackObjects == null) trackObjects = StartCoroutine(TrackObjects());
                break;
            }
        }
    }
    
    private void DestroyThisObject(int idx)
    {
        objectBuffer[idx].distSphere = Vector3.Distance(objectBuffer[idx].transf.position, head.position);
        objectBuffer[idx].disolveSpeedFactor = 1 - Mathf.Clamp((Vector3.Distance(objectBuffer[idx].transf.position, head.position) / objectBuffer[idx].distSphere), 0, 1);
        
        Material objMat = objectBuffer[idx].transf.GetComponent<Renderer>().material;
        if(objMat.HasProperty(isSolidID) && objMat.GetFloat(isSolidID) >0)
        {
            cols[idx] = objMat.GetColor(objColorID) ;
            solidColors[idx] = 1;
            particleMat.SetColorArray(colorID, cols);
        }
        else solidColors[idx] = 0;
        
        particleMat.SetFloatArray(solidID, solidColors);
        Graphics.ConvertTexture(objMat.GetTexture(objTexID), 0, texArray, objectBuffer[idx].offsetInArray);

        objectBuffer[idx].disolve= StartCoroutine( DisolveObject(objectBuffer[idx].offsetInArray, objMat) );
        objectBuffer[idx].particleRun = StartCoroutine( RunParticles(objectBuffer[idx].offsetInArray) );

    }
   

    /**************************************************************************/
    
    private IEnumerator DisolveObject( int idx, Material objMat)
    {
        while(objectBuffer[idx].disolveLevel < 1)
        {
            objectBuffer[idx].disolveSpeedFactor = 1 - Mathf.Clamp((Vector3.Distance(objectBuffer[idx].transf.position, head.position) / objectBuffer[idx].distSphere), 0, 1);
            objectBuffer[idx].disolveLevel += objectBuffer[idx].disolveSpeedFactor /100;
            objMat.SetFloat(disolveLevelID, objectBuffer[idx].disolveLevel);          
            yield return null;
        }

        if (objectBuffer[idx].particleRun != null) StopCoroutine(objectBuffer[idx].particleRun);
        objectBuffer[idx].fadeoff= StartCoroutine(FadeParticles(idx));

        bufferData[idx].active = 0;
        Transform buff = objectBuffer[idx].transf.parent;
        if (buff == null) Destroy(objectBuffer[idx].transf.gameObject);
        else Destroy(buff.gameObject);
        StopCoroutine(objectBuffer[idx].disolve);

        yield return null;
    } //main process

    private IEnumerator RunParticles(int idx)
    {
        //print(objectBuffer[idx].offsetInArray + "----" + idx + "-----" + disolveBuffer[idx].uv.z);
        int[] triangles = objectBuffer[idx].mesh.triangles;
        Vector3[] verts = objectBuffer[idx].mesh.vertices;
        int k = objectBuffer[idx].offsetInParticles;
        int max = Mathf.Min( objectBuffer[idx].offsetInParticles + ( particleCount / maxObjects), particleCount);

        while (k <  max)
        {
            int jump = k + Mathf.FloorToInt(objectBuffer[idx].disolveSpeedFactor * 2000 * objectBuffer[idx].disolveLevel);
            int clampedJump = Mathf.Clamp(jump, 0, max);
            for (int i = k; i < clampedJump; i++)
            {
                int randomIdx = Mathf.FloorToInt(Random.Range(0, triangles.Length / 3 - 1));
                Vector3[] triangleVerts = new Vector3[]
                {
                objectBuffer[idx].transf.TransformPoint(verts[triangles[randomIdx]]),
                objectBuffer[idx].transf.TransformPoint(verts[triangles[randomIdx + 1]]),
                objectBuffer[idx].transf.TransformPoint(verts[triangles[randomIdx + 2]])
                };
                disolveBuffer[i].originVertex = RandomPointOnMesh(triangleVerts)- objectBuffer[idx].transf.position;
                Vector2 uv = objectBuffer[idx].mesh.uv[randomIdx];
                disolveBuffer[i].uv = new Vector3(uv.x, uv.y, idx);
                disolveBuffer[i].position = objectBuffer[idx].transf.position + disolveBuffer[i].originVertex;
                disolveBuffer[i].life = Random.value * 3;
                
            }
            computeBuffer.SetData(disolveBuffer, k, k, max - k);
            k = jump;
            yield return null;
        }
        yield return null;
    } //particle creation

    private IEnumerator RunParticles()
    {
        ParticlesDisolve[] buff = new ParticlesDisolve[particleCount];
        int idx = 0;
        while (idx <= maxObjects)
        {
            int jump = (idx+1) * particleCount / maxObjects;
            int clampedJump = Mathf.Clamp(jump, 0, buff.Length);
            for (int i= idx* particleCount / maxObjects; i<clampedJump; i++)
            {
                buff[idx].originVertex = Vector3.zero;
                buff[idx].position = Vector3.zero;
                buff[idx].life = 3;
                buff[idx].uv = new Vector3(0, 0, idx);
            }
            idx++;
            yield return null;
        }

        disolveBuffer = buff;
        computeBuffer.SetData(disolveBuffer);
        yield return null;
    }  //initial setup overflow
   
    private IEnumerator FadeParticles(int idx)
    {

        while (objectBuffer[idx].disolveLevel > 0 )
        {
            objectBuffer[idx].disolveLevel -= 0.005f;
            stormShader.Dispatch(ComputeKernelID, warpCount, 1, 1);
            yield return null;
        }
        objectBuffer[idx].disolveLevel = 0;
        yield return null;
    } //last stage, item destroyed but leaving time to particles to fade

    private IEnumerator TrackObjects()
    {
        bool isActive = true;
        while(isActive)
        {
            bool atLeastOne = false;
            for(int i=0; i< objectBuffer.Length; i++)
            {

                if(objectBuffer[i].disolveLevel > 0 || bufferData[i].active == 1)
                {
                    atLeastOne = true;
                }
                if(bufferData[i].active == 1)
                {
                    bufferData[i].objectsPosition = objectBuffer[i].transf.position;
                }
            }
            bufferDataBuffer.SetData(bufferData);
            stormShader.Dispatch(ComputeKernelID, warpCount, 1, 1);
            isActive = atLeastOne;
            yield return null;
        }
        StopCoroutine(trackObjects);
        yield return null;
    } //background process checking for activity, shuts down computing when idle

    /**************************************************************************/
    private static Vector3 RandomPointOnMesh(Vector3[] _triangleVerts)
    {
        float rndA = Random.value;
        float rndB = Random.value;
        float rndC = Random.value;

        Vector3 pointOnMesh = (rndA * _triangleVerts[0] + rndB * _triangleVerts[1] + rndC * _triangleVerts[2]) / (rndA + rndB + rndC);
        return pointOnMesh;
    }

    private void GetIDs()
    {
        disolvingID = Shader.PropertyToID("disolving");
        colorID = Shader.PropertyToID("_Colors");
        objColorID = Shader.PropertyToID("Color_5A780C18");
        solidID = Shader.PropertyToID("_Solid");
        isSolidID = Shader.PropertyToID("Vector1_F7ADE39D");
        objTexID= Shader.PropertyToID("Texture2D_EC04F85E");
        disolveLevelID = Shader.PropertyToID("Vector1_2465EC98");
    }


    void OnDestroy()
    {
        if (computeBuffer != null)
            computeBuffer.Release();
        if (bufferDataBuffer != null)
            bufferDataBuffer.Release();
    }
    
}
