using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class SceneReference : MonoBehaviour
{
#if UNITY_EDITOR
    public UnityEditor.SceneAsset sceneAsset;
#endif

    [SerializeField]
    float loadDelayIfWebGL = 0f;

    [SerializeField]
    UnityEvent<string> OnBeforeLoad;

    [HideInInspector]
    public string scenePath;

    [@System.Obsolete]
    public void Load()
    {
        LoadImmediate();
    }

    public void LoadImmediate()
    {
        if (scenePath == "")
        {
            Debug.LogWarning("SceneReference does not have a path");
            return;
        }

        OnBeforeLoad.Invoke(scenePath);

#if UNITY_EDITOR
        EditorSceneManager.LoadSceneInPlayMode(
            scenePath,
            new LoadSceneParameters(LoadSceneMode.Single)
        );
#else
        SceneManager.LoadScene(scenePath);
#endif
    }

    public void LoadAsync(bool skipDelayIfWebGL = false)
    {
        if (scenePath == "")
        {
            Debug.LogWarning("SceneReference does not have a path");
            return;
        }

        OnBeforeLoad.Invoke(scenePath);

        if (IsWebGL() && !skipDelayIfWebGL && loadDelayIfWebGL > 0f)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(loadDelayIfWebGL));
        }
        else
        {
            LoadSceneAsync();
        }
    }

    AsyncOperation LoadSceneAsync()
    {
#if UNITY_EDITOR
        return EditorSceneManager.LoadSceneAsyncInPlayMode(
            scenePath,
            new LoadSceneParameters(LoadSceneMode.Single)
        );
#else
        return SceneManager.LoadSceneAsync(scenePath);
#endif
    }

    IEnumerator LoadSceneAsyncCoroutine(float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        LoadSceneAsync();
    }

    bool IsWebGL()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }
}
