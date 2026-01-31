using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkPlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // assign in inspector
    private void Awake()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport == null) transport = NetworkManager.Singleton.gameObject.AddComponent<UnityTransport>();

        // Ensure a valid IP and non-zero port for listening
        transport.SetConnectionData(forceOverrideCommandLineArgs: true, ipv4Address: "127.0.0.1", port: 7777, listenAddress: "0.0.0.0");
    }
    private void Start()
    {
        // Safety checks
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found in scene!");
            return;
        }

        // Register callback for clients connecting
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        // Spawn host player once
        if (NetworkManager.Singleton.IsHost)
        {
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only server spawns players
        if (!NetworkManager.Singleton.IsServer) return;

        // Prevent double-spawn for host
        if (clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost)
            return;

        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        // Prevent spawning if player already exists for this client
        foreach (var netObj in FindObjectsOfType<NetworkObject>())
        {
            if (netObj.IsPlayerObject && netObj.OwnerClientId == clientId)
            {
                Debug.Log($"Player for client {clientId} already exists, skipping spawn.");
                return;
            }
        }

        // Instantiate and spawn
        GameObject player = Instantiate(playerPrefab);
        var playerNetObj = player.GetComponent<NetworkObject>();
        if (playerNetObj == null)
        {
            Debug.LogError("Player prefab missing NetworkObject!");
            return;
        }

        playerNetObj.SpawnAsPlayerObject(clientId);
        Debug.Log($"Spawned player for client {clientId}");
    }
}