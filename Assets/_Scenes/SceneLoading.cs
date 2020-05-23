using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    public bool active = false;
    private PlayerBoundaries playerbounds;
    private MeshRendererLoading[] loadings;
    private Coroutine waitToUnload;
    private string[] rooms;

    private void Start()
    {
        GameObject[] buff = GameObject.FindGameObjectsWithTag("LoadMesh");
        loadings = new MeshRendererLoading[buff.Length];
        rooms = new string[buff.Length];
        for (int i = 0; i < buff.Length; i++)
        {
            loadings[i] = buff[i].GetComponent<MeshRendererLoading>();
            rooms[i] = buff[i].name;
        }  
    }

    public void LoadThisScene(string _scene)
    {
        for(int i=0; i<loadings.Length; i++)
        {
            if(rooms[i] == _scene + "_LoadMesh")
            {
                if (waitToUnload != null) StopCoroutine(waitToUnload);
                loadings[i].LoadRenderers(true);
                break;
            }
        }
    }

    public void UnloadThisScene(string _scene)
    {
        for (int i = 0; i < loadings.Length; i++)
        
           
            if (rooms[i] == _scene + "_LoadMesh")
            {
                if (_scene != playerbounds.currentZone)
                    loadings[i].LoadRenderers(false);
                else waitToUnload = StartCoroutine(WaitToUnload(_scene, i));
                break;
            }
        
            
    }

    private IEnumerator WaitToUnload(string _zone, int idx)
    {
        while(_zone != playerbounds.currentZone)
        {
            yield return null;
        }
        loadings[idx].LoadRenderers(false);
    }

    /*
    public void LoadThisScene(string _scene)
    {
        if (active && !SceneManager.GetSceneByName(_scene).IsValid())
            StartCoroutine(LoadScene(_scene));
    }

    public void UnloadThisScene(string _scene)
    {
        if (active && playerbounds.currentZone != _scene && SceneManager.GetSceneByName(_scene).IsValid())
            StartCoroutine(UnLoadScene(_scene));
    }

    
    private IEnumerator LoadScene(string _scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_scene, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator UnLoadScene(string _scene)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(_scene);

        while (asyncUnload != null || !asyncUnload.isDone)
        {
            yield return null;
        }
    }
    */
}
