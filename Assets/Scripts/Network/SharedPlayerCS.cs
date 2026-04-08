using Unity.Netcode;
using UnityEngine;

public class SharedPlayerCS : MonoBehaviour
{
     #region Network Role Assignment

    public NetworkVariable<ulong> legsPlayerId =
        new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<ulong> upperPlayerId =
        new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

    #endregion
}
