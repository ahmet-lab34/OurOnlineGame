using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject LookingForLobby;
    [SerializeField] private GameObject InsideLobby;
    [SerializeField] private GameObject HostInsideLobby;

    [Header("Player Containers")]
    [SerializeField] private Transform hostPlayerContainer;
    [SerializeField] private Transform normalPlayerContainer;

    [Header("Player Prefabs")]
    [SerializeField] private GameObject hostPlayerListPrefab;
    [SerializeField] private GameObject normalPlayerListPrefab;

    [Header("Lobby Data")]
    public Lobby hostLobby;        // If we are hosting
    public Lobby joinedLobby;      // Lobby we are in

    private void RefreshLobbyUI()
    {
        if (hostLobby != null)
        {
            // We are the host
            HostInsideLobby.SetActive(true);
            InsideLobby.SetActive(false);
            LookingForLobby.SetActive(false);

            RefreshPlayerList(hostLobby.Players, hostPlayerContainer, hostPlayerListPrefab);
        }
        else if (joinedLobby != null)
        {
            // Normal player
            InsideLobby.SetActive(true);
            HostInsideLobby.SetActive(false);
            LookingForLobby.SetActive(false);

            RefreshPlayerList(joinedLobby.Players, normalPlayerContainer, normalPlayerListPrefab);
        }
        else
        {
            // Not in a lobby yet
            HostInsideLobby.SetActive(false);
            InsideLobby.SetActive(false);
            LookingForLobby.SetActive(true);

            // Clear containers
            ClearContainer(hostPlayerContainer);
            ClearContainer(normalPlayerContainer);
        }
    }


    private void RefreshPlayerList(List<Player> players, Transform container, GameObject prefab)
    {
        ClearContainer(container);
        Debug.Log($"RefreshPlayerList called with {players.Count} players");

        foreach (var player in players)
        {
            Debug.Log($"Processing player: {player.Id}");
            Debug.Log($"Player.Data is null: {player.Data == null}");
            
            GameObject slotGO = Instantiate(prefab, container);
            
            PlayerNameSlot slotUI = slotGO.GetComponent<PlayerNameSlot>();
            if (slotUI == null)
            {
                Debug.LogError($"PlayerNameSlot component not found on prefab: {prefab.name}");
                continue;
            }

            string playerDisplayName = "Unknown Player";
            
            // Try to get the player name from data
            if (player.Data != null && player.Data.ContainsKey("PlayerName"))
            {
                var playerNameObj = player.Data["PlayerName"];
                if (playerNameObj != null && playerNameObj.Value != null)
                {
                    playerDisplayName = playerNameObj.Value;
                    Debug.Log($"Found PlayerName for {player.Id}: {playerDisplayName}");
                }
            }
            else
            {
                Debug.LogWarning($"PlayerName data not found for player {player.Id}");
                if (player.Data != null)
                {
                    Debug.Log($"Player data keys: {string.Join(", ", player.Data.Keys)}");
                }
            }
            
            slotUI.SetPlayer(playerDisplayName);
        }
    }
    

    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }

    // Call this whenever the lobby updates
    public void OnLobbyHosted(Lobby lobby)
    {
        hostLobby = lobby; // Host lobby is the same as joined lobby
        RefreshLobbyUI();
    }
    public void OnLobbyJoined(Lobby lobby)
    {
        joinedLobby = lobby; // For simplicity, we treat joined lobby as host lobby in this example
        RefreshLobbyUI();
    }
}