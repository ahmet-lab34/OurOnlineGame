using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private TMP_Text gameModeText;
    [SerializeField] private Button joinButton;

    private Lobby lobby;
    private TestRelay lobbyManager;

    public void SetLobby(Lobby lobby, TestRelay manager)
    {
        this.lobby = lobby;
        this.lobbyManager = manager;

        lobbyNameText.text = lobby.Name;
        playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
        gameModeText.text = lobby.Data["GameMode"].Value;

        joinButton.interactable = lobby.Players.Count < lobby.MaxPlayers;
        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() =>
        {
            lobbyManager.JoinLobbyById(lobby.Id);
        }
        );
    }
}
