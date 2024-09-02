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
    UnityEvent<string> OnBeforeLoad;

    [HideInInspector]
    public string scenePath;

    public void Load()
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

    public AsyncOperation LoadAsync()
    {
        if (scenePath == "")
        {
            Debug.LogWarning("SceneReference does not have a path");
            return null;
        }

        OnBeforeLoad.Invoke(scenePath);

#if UNITY_EDITOR
        return EditorSceneManager.LoadSceneAsyncInPlayMode(
            scenePath,
            new LoadSceneParameters(LoadSceneMode.Single)
        );
#else
        return SceneManager.LoadSceneAsync(scenePath);
#endif
    }

    /// <summary>
    /// Alias for LoadAsync to make it possible to call from Unity events (on click etc).
    /// </summary>
    public void LoadBackground()
    {
        LoadAsync();
    }
}
