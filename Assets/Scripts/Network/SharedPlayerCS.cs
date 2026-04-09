using Unity.Netcode;
using UnityEngine;

public class SharedPlayerCS : NetworkBehaviour
{
    #region Network Role Assignment

    public NetworkVariable<ulong> legsPlayerId =
        new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<ulong> upperPlayerId =
        new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

    #endregion

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            legsPlayerId.OnValueChanged += OnRolesChanged;
            upperPlayerId.OnValueChanged += OnRolesChanged;

            OnRolesChanged(0, 0); // Force initial check
        }
    }

    private void OnRolesChanged(ulong oldValue, ulong newValue)
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

    // ✅ Helper functions (used by other scripts)

    public bool IsLegsPlayer(ulong clientId)
    {
        return clientId == legsPlayerId.Value;
    }

    public bool IsUpperPlayer(ulong clientId)
    {
        return clientId == upperPlayerId.Value;
    }

    // ✅ Server assigns roles
    public void AssignRolesServer(ulong legsClientId, ulong upperClientId)
    {
        if (!IsServer) return;

        legsPlayerId.Value = legsClientId;
        upperPlayerId.Value = upperClientId;
    }
}