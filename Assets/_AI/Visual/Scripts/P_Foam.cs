using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Foam : MonoBehaviour
{

    /****
    **********/
    #region particles
    private struct Particles
    {
        public Vector3 position;
        public Vector3 offSet;
        public Vector3 dir;
    }

    Particles[] particleBuffer;
    #endregion


    /****
    **********/
    #region compute data
    private const int WARP_SIZE = 512;
    private const int PS_DATA_SIZE = 36;
    private int warpCount;
    private int ComputeKernelID;

    private ComputeBuffer computeBuffer;
    #endregion


    /****
    **********/
    #region Externals
    private ShaderInputs inputs;

    private Material particleMat;
    private int particleCount;
    private ComputeShader computeShader;
    private Transform headPosition;
    #endregion


    /***********************************************************/

    private void Awake()
    {
        headPosition = GameObject.FindWithTag("Agent_Head").transform;
        inputs = GetComponent<ShaderInputs>();

        computeShader= inputs.computeShader;
        particleMat = inputs.particleMaterial;
        particleCount = inputs.particleCounts;
    }

    private void Start()
    {
        warpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);
        ComputeKernelID = computeShader.FindKernel("CSFoam");

        //particleBuffer = StartupParticles(particleCount);

        computeBuffer = new ComputeBuffer(particleCount, PS_DATA_SIZE);
        computeBuffer.SetData(particleBuffer);

        computeShader.SetBuffer(ComputeKernelID, "particleBuffer", computeBuffer);
        particleMat.SetBuffer("particleBuffer", computeBuffer);
    }
}
