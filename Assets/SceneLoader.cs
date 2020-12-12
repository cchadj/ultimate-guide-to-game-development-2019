using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [ContextMenu("Load next level")]
    public void LoadNextLevel()
    {
        LoadLevel(1);
    }
    
    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
       var loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);

       while (!loadingOperation.isDone)
       {
           var progress = Mathf.Clamp01(loadingOperation.progress / .9f);
           yield return null;
       }
    }
}
