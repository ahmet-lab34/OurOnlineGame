using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [Header("Optional: Name of the persistent first scene")]
    [SerializeField] private string persistentSceneName = "FirstScene";

    [Header("List of all scene names (optional, for index-based loading)")]
    [SerializeField] private List<string> sceneNames = new List<string>();

    private void Awake()
    {
        // Make the SceneManager persistent
        DontDestroyOnLoad(gameObject);
    }

    #region Public Load Methods

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }

        // Directly load the scene in Single mode
        if (sceneName != persistentSceneName)
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError($"Scene '{sceneName}' is not in Build Settings!");
            }
        }
    }

    public void LoadSceneByIndex(int index)
    {
        if (index < 0 || index >= sceneNames.Count)
        {
            Debug.LogError("Scene index out of range!");
            return;
        }

        LoadSceneByName(sceneNames[index]);
    }

    #endregion
}