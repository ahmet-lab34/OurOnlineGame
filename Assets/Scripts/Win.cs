using UnityEngine;

public class Win : MonoBehaviour
{
    public GameObject WinningPrize;
    void OnTriggerEnter2D(Collider2D other)
    {
        WinningPrize.SetActive(true);
        Time.timeScale = 0f;
    }
}
