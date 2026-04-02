using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDecide : MonoBehaviour
{
    [SerializeField] private GameObject sharedCharacterPrefab;

    private List<ulong> waitingPlayers = new();

    private void Start()
    {
        Debug.Log("PlayerDecide started, waiting for players to connect...");
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only the server handles spawning
        if (!NetworkManager.Singleton.IsServer) return;

        waitingPlayers.Add(clientId);
        Debug.Log($"[Server] Player {clientId} connected. Total waiting players: {waitingPlayers.Count}");

        // Spawn a shared character for every 2 players
        if (waitingPlayers.Count >= 2)
        {
            Debug.Log("[Server] Two players connected, spawning shared character.");

            ulong playerA = waitingPlayers[0];
            ulong playerB = waitingPlayers[1];

            waitingPlayers.RemoveRange(0, 2);

            SpawnSharedCharacter(playerA, playerB);
        }
    }

    private void SpawnSharedCharacter(ulong legsId, ulong upperId)
    {
        // Instantiate the prefab
        var obj = Instantiate(sharedCharacterPrefab);
        var netObj = obj.GetComponent<NetworkObject>();
        netObj.Spawn(); // This replicates to all clients automatically

        // Assign controlling client IDs
        var script = obj.GetComponent<PlayerScript>();
        script.legsPlayerId.Value = legsId;
        script.upperPlayerId.Value = upperId;

        Debug.Log($"[Server] Spawned shared character. Legs: {legsId}, Upper: {upperId}");
    }
}