using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererLoading : MonoBehaviour
{
    [SerializeField] string SceneName;
    private List<MeshRenderer> allmeshes = new List<MeshRenderer>();

    private void Awake()
    {
        Transform[] go = GameObject.FindObjectsOfType(typeof(Transform)) as Transform[];
        for (int i = 0; i < go.Length; i++)
        {
            if (go[i].GetComponent<MeshRenderer>() != null && go[i].gameObject.scene.name == SceneName)
            {
                allmeshes.Add(go[i].GetComponent<MeshRenderer>());
            }
            foreach (MeshRenderer m in allmeshes) m.enabled = false;
        }
    }

    public void LoadRenderers(bool _load)
    {
        foreach (MeshRenderer m in allmeshes) m.enabled = _load;
    }
}
