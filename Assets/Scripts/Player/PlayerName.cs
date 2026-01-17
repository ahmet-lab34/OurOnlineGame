using Mirror;
using UnityEngine;

public class PlayerIdentity : NetworkBehaviour
{
    [SyncVar] public string playerName;

    public override void OnStartLocalPlayer()
    {
        playerName = "Player_" + Random.Range(0, 999);
        CmdSetName(playerName);
    }

    [Command]
    void CmdSetName(string name)
    {
        playerName = name;
    }
}
