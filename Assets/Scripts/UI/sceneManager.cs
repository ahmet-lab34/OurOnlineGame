using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    [SerializeField] private List<string> sceneNames = new List<string>();

    public void LoadSceneByIndex(int index)
    {
        if (index < 0 || index >= sceneNames.Count)
        {
            Debug.LogError("Scene index out of range!");
            return;
        }

        SceneManager.LoadScene(sceneNames[index]);
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
