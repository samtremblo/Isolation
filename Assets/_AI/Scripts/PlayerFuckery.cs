using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using ImageEffectGraph.PostProcessing;
using ImageEffectGraph.Editor.PostProcessing;

public class PlayerFuckery : MonoBehaviour
{

    private int screenPosID;
    private int colliderLayerMask;

    private Camera cam;
    private Transform agent;
    private Transform player;
    private bool inZone = false;
    private Brain agentBrain;

    private RenderWithMaterial profileCustom;

    private void Awake()
    {
        colliderLayerMask = 1 << 9;

        player = GameObject.FindWithTag("MainCamera").transform;
        agent = GameObject.FindWithTag("Agent_Head").transform;
        cam = player.GetComponent<Camera>();

        PostProcessVolume pp = FindObjectOfType<PostProcessVolume>();
        pp.profile.TryGetSettings(out profileCustom);

        screenPosID = Shader.PropertyToID("_ScreenPos");
        profileCustom.material.value.SetFloat("Vector1_ECA003F1", 0);

    }

    private void Update()
    {
        
        if (!inZone)
        {
            //profileCustom.material.value.SetFloat("Vector1_ECA003F1", Time.time);
            //print(profileCustom.material.value.GetFloat("Vector1_ECA003F1"));
            //mat.SetFloat("Vector1_ECA003F1", Time.time);
            //profileCustom.material.Override(mat);

            /*
            print(mat.GetVector(screenPosID));
            Vector3 screenPos = cam.WorldToViewportPoint(agent.position);
            if (screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1 && screenPos.z > 0)
                if (Physics.Raycast(player.position, agent.position - player.position, 100, colliderLayerMask))
                    mat.SetVector(screenPosID, screenPos);
                else mat.SetVector(screenPosID, Vector3.zero);
            else mat.SetVector(screenPosID, Vector3.zero);
            */
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "HeadCollider")
        {
            if(agentBrain.AgressiveLevel < 5) agentBrain.AgressiveLevel = agentBrain.AgressiveLevel + 1;
            inZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "HeadCollider")
        {
            inZone = false;
        }
    }
}
