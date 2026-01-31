using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadUIScene : MonoBehaviour
{
    public string sceneName;

    public SceneManagerScript sceneManager;

    private void Awake()
    {
        SceneManager.LoadScene(sceneName);
    }
    private void Start()
    {
        Destroy(gameObject);
    }
}
