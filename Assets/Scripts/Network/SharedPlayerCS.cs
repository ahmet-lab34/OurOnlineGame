using Unity.Netcode;
using UnityEngine;

public class SharedPlayerCS : NetworkBehaviour
{
    #region Network Role Assignment

    // Server-authoritative player IDs
    public NetworkVariable<ulong> legsPlayerId =
        new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<ulong> upperPlayerId =
        new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

    #endregion

    private Player_1_Actions player1Actions;
    private Player_2_Actions player2Actions;

    public override void OnNetworkSpawn()
    {
        // Only run this on clients
        if (!IsClient) return;

        // Initialize input actions
        player1Actions = new Player_1_Actions();
        player2Actions = new Player_2_Actions();

        // Subscribe to NetworkVariable changes to safely assign roles once synced
        legsPlayerId.OnValueChanged += OnLegsPlayerIdChanged;
        upperPlayerId.OnValueChanged += OnUpperPlayerIdChanged;

        // Also check current values in case they were already synced
        HandleRoles(NetworkManager.Singleton.LocalClientId);
    }

    private void OnLegsPlayerIdChanged(ulong oldValue, ulong newValue)
    {
        HandleRoles(NetworkManager.Singleton.LocalClientId);
    }

    private void OnUpperPlayerIdChanged(ulong oldValue, ulong newValue)
    {
        HandleRoles(NetworkManager.Singleton.LocalClientId);
    }

    /// <summary>
    /// Assigns input control based on the local client ID
    /// </summary>
    private void HandleRoles(ulong localClientId)
    {
        // Only assign once
        if (player1Actions == null || player2Actions == null) return;

        if (localClientId == legsPlayerId.Value)
        {
            Debug.Log("This client controls the legs.");
            player1Actions.Upper.Disable();
            player2Actions.Lower.Enable();
        }
        else if (localClientId == upperPlayerId.Value)
        {
            Debug.Log("This client controls the upper body.");
            player1Actions.Upper.Enable();
            player2Actions.Lower.Disable();
        }
        else
        {
            Debug.LogWarning("This client does not control any part of the character.");
            player1Actions.Upper.Disable();
            player2Actions.Lower.Disable();
        }
    }

    /// <summary>
    /// Server-only helper to assign roles before spawning
    /// </summary>
    public void AssignRolesServer(ulong legsClientId, ulong upperClientId)
    {
        if (!IsServer) return;

        legsPlayerId.Value = legsClientId;
        upperPlayerId.Value = upperClientId;
    }
}