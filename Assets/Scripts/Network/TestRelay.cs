using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;

public class TestRelay : MonoBehaviour
{
    private LobbyUIManager lobbyUIManager;
    [SerializeField] private GameObject playerPrefab;

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float refreshLobbyListTimer = 5f;
    private float heartbeatTimer;
    private float clientLobbyUpdateTimer;
    private float hostLobbyUpdateTimer;
    private bool hasJoinedRelay = false;
    private const string RELAY_CODE_KEY = "RelayJoinCode";

    [SerializeField] private string playerName;
    [SerializeField] private TMP_InputField lobbyCodeInput;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private InputField filterByNameInput;
    [SerializeField] private Transform lobbyListContent;
    [SerializeField] private GameObject lobbyListItemPrefab;

    private List<GameObject> lobbyItems = new();

    #region Unity Callbacks
    private async void Start()
    {
        // Initialize Unity Services only once
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        // Sign in only if not already signed in
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}");
        };

        // Default player name
        if (string.IsNullOrEmpty(playerName))
            playerName = "Ahmet" + Random.Range(10, 99);

        Debug.Log(playerName);

        lobbyUIManager = GetComponent<LobbyUIManager>();

        LateStart();
    }
    private void LateStart()
    {
        CreateRelay();
    }

    private void Update()
    {
        //HandleRefreshLobbyList(); // auto-refresh lobby list
        HandleLobbyHeartbeat();
        _ = HandleClientLobbyPollForUpdates();
        _ = HandleHostLobbyPollForUpdates();
    }
    #endregion

    #region Authentication
    public async void Authenticate(string playerName)
    {
        this.playerName = playerName;
        Debug.Log($"Signed in as {playerName} | Player ID: {AuthenticationService.Instance.PlayerId}");
    }

    public void AuthenticateButton()
    {
        string nameInput = playerNameInput.text;
        if (!string.IsNullOrEmpty(nameInput))
            playerName = nameInput;
        else
            playerName = "Player" + Random.Range(10, 99);

        Authenticate(playerName);
    }
    #endregion

    #region Lobby Management
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

            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            joinedLobby = hostLobby;

            Debug.Log($"Created Lobby! {hostLobby.Name} - {hostLobby.MaxPlayers} - {hostLobby.Id} - {hostLobby.LobbyCode}");

            PrintPlayers(hostLobby);
            lobbyUIManager.OnLobbyHosted(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryOptions = new QueryLobbiesOptions
            {
                Count = 10,
                Filters = new List<QueryFilter>()
            };

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
            Debug.Log($"Found {response.Results.Count} lobbies.");

            foreach (GameObject item in lobbyItems)
                Destroy(item);
            lobbyItems.Clear();

            foreach (Lobby lobby in response.Results)
            {
                GameObject item = Instantiate(lobbyListItemPrefab, lobbyListContent);
                item.GetComponent<LobbyListItemUI>().SetLobby(lobby, this);
                lobbyItems.Add(item);

                Debug.Log($"Lobby: {lobby.Name} - {lobby.MaxPlayers} max players - GameMode: {lobby.Data["GameMode"].Value}");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions { Player = GetPlayer() };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);

            joinedLobby = lobby;
            hostLobby = null;

            Debug.Log($"Joined Lobby! {lobbyCode}");
            PrintPlayers(joinedLobby);
            lobbyUIManager.OnLobbyJoined(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinLobbyById(string lobbyId)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions { Player = GetPlayer() };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);

            joinedLobby = lobby;
            hostLobby = null;

            Debug.Log($"Joined lobby {joinedLobby.Name} successfully!");
            PrintPlayers(joinedLobby);
            lobbyUIManager.OnLobbyJoined(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to join lobby: {e}");
        }
    }

    public async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
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

    public void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log($"Players in Lobby {lobby.Name} {lobby.Data["GameMode"].Value}:");
        foreach (Player player in lobby.Players)
        {
            Debug.Log($"Player: {player.Id} - {player.Data["PlayerName"].Value}");
        }
    }

    public async Task UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode, DataObject.IndexOptions.S1) }
                }
            });
            joinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task UpdatePlayerName(string newPlayerName)
    {
        playerName = newPlayerName;
        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
                }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[2].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });
            joinedLobby = hostLobby;
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private void HandleRefreshLobbyList()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f)
            {
                refreshLobbyListTimer = 5f;
                ListLobbies();
            }
        }
    }
    #endregion

    #region Relay Management
    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            hasJoinedRelay = true;

            if (hostLobby != null)
                await UpdateLobbyRelayCode(joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinRelay(string joinCode)
    {
        if (hasJoinedRelay) return;

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            hasJoinedRelay = true;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async Task UpdateLobbyRelayCode(string relayCode)
    {
        try
        {
            hostLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { RELAY_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Public, relayCode) }
                }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to update lobby with relay code: {e}");
        }
    }
    #endregion

    #region Lobby Heartbeat & Polling
    private void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
            LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            
    }

    private async Task HandleClientLobbyPollForUpdates()
    {
        if (joinedLobby == null || hostLobby != null) return; // Only clients poll this

        clientLobbyUpdateTimer -= Time.deltaTime;
        if (clientLobbyUpdateTimer <= 0f)
        {
            clientLobbyUpdateTimer = 1.1f;

            try
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;

                // Check for relay join code
                if (!hasJoinedRelay && joinedLobby.Data != null && joinedLobby.Data.ContainsKey(RELAY_CODE_KEY))
                {
                    string relayCode = joinedLobby.Data[RELAY_CODE_KEY].Value;
                    JoinRelay(relayCode);
                }

                // Optionally update client UI
                lobbyUIManager.OnLobbyJoined(joinedLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning($"Client lobby poll failed: {e.Message}");
            }
        }
    }

    private async Task HandleHostLobbyPollForUpdates()
    {
        if (hostLobby == null) return; // Only host polls this

        hostLobbyUpdateTimer -= Time.deltaTime;
        if (hostLobbyUpdateTimer <= 0f)
        {
            hostLobbyUpdateTimer = 1.1f;

            try
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(hostLobby.Id);
                hostLobby = lobby;

                // Update host UI with latest player list
                PrintPlayers(hostLobby);
                lobbyUIManager.OnLobbyHosted(hostLobby); // Make sure this method refreshes the UI
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning($"Host lobby poll failed: {e.Message}");
            }
        }
    }
    #endregion
}