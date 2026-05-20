using TMPro;
using UnityEngine;

public class PlayerNameSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;

    public void SetPlayer(string playerName)
    {
        if (playerNameText == null)
        {
            // Try to find the text field automatically
            playerNameText = GetComponentInChildren<TMP_Text>();
            if (playerNameText == null)
            {
                Debug.LogError("PlayerNameSlot: No TMP_Text component found! Please assign it in the prefab.");
                return;
            }
        }
        
        playerNameText.text = playerName;
        Debug.Log($"Set player name to: {playerName}");
    }
}
