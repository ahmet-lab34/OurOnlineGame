using Unity.Netcode;
using UnityEngine;

public class SharedPlayerCS : NetworkBehaviour
{
    public NetworkVariable<ulong> legsPlayerId =
        new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<ulong> upperPlayerId =
        new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

    private PlayerController playerController;
    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        legsPlayerId.OnValueChanged += (_, __) => RefreshRoleState();
        upperPlayerId.OnValueChanged += (_, __) => RefreshRoleState();

        RefreshRoleState(); // correct initial sync
    }

    public override void OnNetworkDespawn()
    {
        legsPlayerId.OnValueChanged -= (_, __) => RefreshRoleState();
        upperPlayerId.OnValueChanged -= (_, __) => RefreshRoleState();
    }

    private void RefreshRoleState()
    {
        ulong localId = NetworkManager.Singleton.LocalClientId;

        if (localId == legsPlayerId.Value)
        {
            Debug.Log("This client controls the LEGS.");
        }
        else if (localId == upperPlayerId.Value)
        {
            Debug.Log("This client controls the UPPER BODY.");
        }
        else
        {
            Debug.Log("This client is a spectator.");
        }
    }

    public bool IsLegsPlayer(ulong clientId) =>
        clientId == legsPlayerId.Value;

    public bool IsUpperPlayer(ulong clientId) =>
        clientId == upperPlayerId.Value;

    public void AssignRolesServer(ulong legsClientId, ulong upperClientId)
    {
        if (!IsServer) return;

        if (legsClientId == upperClientId)
        {
            Debug.LogError("Cannot assign same client to both roles.");
            return;
        }

        legsPlayerId.Value = legsClientId;
        upperPlayerId.Value = upperClientId;
    }
}