using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryPanelButtons : MonoBehaviour
{
    public void RestartBossScene()
    {
        SceneManager.LoadScene("BossTestingScene");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("GameUI");
    }
}