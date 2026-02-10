using TMPro;
using UnityEngine;

public class PlayerNameSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;

    public void SetPlayer(string playerName)
    {
        playerNameText.text = playerName;
    }
}
