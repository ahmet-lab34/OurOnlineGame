using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;

public class TestRelay : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();


        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Ahmet" + Random.Range(10, 99);

        Debug.Log(playerName);
    }

    private async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join Code: {joinCode}");

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        } 
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }   
    }

    private async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Attempting to join Relay with join code: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log("Joined Relay successfully.");

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }


    private Lobby hostLobby;
    private float heartbeatTimer;
    private string playerName;

    private void Update()
    {
        HandleLobbyHeartbeat();
    }


    private void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 2;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "CaptureTheFlag", DataObject.IndexOptions.S1) }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            
            hostLobby = lobby;

            Debug.Log($"Created Lobby! {lobby.Name} - {lobby.MaxPlayers} - {lobby.Id} - {lobby.LobbyCode}");

            PrintPlayers(hostLobby);

        } catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 10,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    new QueryFilter(QueryFilter.FieldOptions.S1, "CaptureTheFlag", QueryFilter.OpOptions.EQ)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };


            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log($"Found {response.Results.Count} lobbies.");

            foreach (Lobby lobby in response.Results)
            {
                Debug.Log($"Lobby: {lobby.Name} - {lobby.MaxPlayers} max players {lobby.Data["GameMode"].Value}");
            }
        } catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            Debug.Log($"Joined Lobby! {lobbyCode}");

            PrintPlayers(joinedLobby);
        } catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        } catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
            }
        };
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log($"Players in Lobby {lobby.Name} {lobby.Data["GameMode"].Value}:");
        foreach (Player player in lobby.Players)
        {
            Debug.Log($"Player: {player.Id} - {player.Data["PlayerName"].Value}");
        }
    }
}