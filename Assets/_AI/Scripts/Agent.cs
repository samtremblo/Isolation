using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class Agent : MonoBehaviour
{
    private Vector3 origin = new Vector3(3.4f, 1.4f, 8.3f);
    public Vector3 Origin
    {
        get { return origin; }
    }
    private P_MainStorm agentParticles;
    private Transform head;

    private void Awake()
    {
        head = GameObject.FindWithTag("Agent_Head").transform;
        agentParticles = FindObjectOfType<P_MainStorm>();
    }

    public void InitialSpook(Vector3 _spawnPos)
    {
        head.position = _spawnPos;
        agentParticles.Spawn(true);
    }

    public void DespawnAgent(bool _respawn)
    {
        agentParticles.Despawn();
        if(_respawn) Respawn();
    }

    public void Respawn()
    {
        head.position = origin;
        StartCoroutine(RespawnAgent());
    }

    private IEnumerator RespawnAgent()
    {
        yield return new WaitForSeconds(10);
        agentParticles.Spawn(false);
        yield return null;
    }
}
