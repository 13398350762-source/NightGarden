using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesManager : BaseManager<ScenesManager>
{
    public void LoadScene(string sceneName,UnityAction action)
    {
        SceneManager.LoadScene(sceneName);
        action?.Invoke();
    }

    public void LoadSceneAsync(string sceneName, UnityAction action=null)
    {
        MonoManager.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneName, action));
    }

    private IEnumerator ReallyLoadSceneAsync(string sceneName, UnityAction action=null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            EventCenter.Instance.EventTrigger("Loading", asyncLoad.progress);
            yield return asyncLoad.progress;
        }
        action?.Invoke();
    }
}
