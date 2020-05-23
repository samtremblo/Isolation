using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.LightweightPipeline;

[ExecuteAlways]
[ImageEffectAllowedInSceneView]
public class CustomRenderPass : MonoBehaviour, IAfterOpaquePass
{
    [SerializeField]
    public Material[] particleMaterials;
    [SerializeField]
    private int[] particleCounts;
    private CustomParticulePass drawing;

    public void OnEnable()
    {
        ShaderInputs[] buffscript = GameObject.FindWithTag("Agent_Particles").GetComponentsInChildren<ShaderInputs>();
        particleCounts = new int[buffscript.Length];
        particleMaterials = new Material[buffscript.Length];
        for (int i=0; i< buffscript.Length; i++)
        {
            particleCounts[i] = buffscript[i].particleCounts;
            particleMaterials[i] = buffscript[i].particleMaterial;

        }
        
    }

    ScriptableRenderPass IAfterOpaquePass.GetPassToEnqueue(RenderTextureDescriptor baseDescriptor, RenderTargetHandle colorAttachmentHandle, RenderTargetHandle depthAttachmentHandle)
    {
        if (drawing == null)
            drawing = new CustomParticulePass(particleMaterials, particleCounts);
        return drawing;
    }
}


public class CustomParticulePass : ScriptableRenderPass
{
    const string RenderPassTag = "Compute shader particles pass";
   
    private Material[] mats;
    private int[] instanceCounts;

    public CustomParticulePass(Material[] _mats, int[] _counts)
    {
        instanceCounts = _counts;
        mats = _mats;
    }

    public override void Execute(ScriptableRenderer renderer, ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(RenderPassTag);
        if(instanceCounts.Length > 0  && mats.Length > 0)
        {
            for (int i = 0; i < instanceCounts.Length; i++)
            {
                int pass= mats[i].FindPass("MainPass");
                cmd.DrawProcedural(Matrix4x4.identity, mats[i], pass, MeshTopology.Points, 1, instanceCounts[i]);
                
            }
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
