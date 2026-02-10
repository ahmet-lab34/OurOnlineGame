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


    // ERROR!!!!! 
    private void RefreshPlayerList(List<Player> players, Transform container, GameObject prefab)
    {
        ClearContainer(container);

        foreach (var player in players)
        {
            GameObject slotGO = Instantiate(prefab, container);
            
            PlayerNameSlot slotUI = slotGO.GetComponent<PlayerNameSlot>();
            if (slotUI != null && player.Data.ContainsKey("PlayerName"))
            {
                slotUI.SetPlayer(player.Data["PlayerName"].Value);
            }
            else if (slotUI == null)
            {
                Debug.LogError($"PlayerNameSlot component not found on prefab: {prefab.name}");
            }
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