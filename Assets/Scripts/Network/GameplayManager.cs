using UnityEngine;
using Unity.Netcode;

public class GameplayManager : MonoBehaviour
{
    public GameObject playerPrefab;

    public void SpawnAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (!client.PlayerObject.IsSpawned)
            {
                var playerInstance = Instantiate(playerPrefab);
                playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId);
            }
        }
    }
}
