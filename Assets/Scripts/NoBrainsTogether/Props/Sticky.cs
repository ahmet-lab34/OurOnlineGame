using Unity.Netcode;
using UnityEngine;

public class Sticky : NetworkBehaviour
{
    [SerializeField] private RagdollController ragdollController;

    private float originalJumpForce;
    private float originalSpeed;

    private void Awake()
    {
        if (ragdollController == null)
            ragdollController = GetComponentInParent<RagdollController>();

        originalSpeed = ragdollController.moveSpeed;
        originalJumpForce = ragdollController.jumpForce;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return; // Only server handles physics

        if (collision.CompareTag("Player"))
        {
            ulong clientId = collision.GetComponentInParent<NetworkObject>().OwnerClientId;
            SetStickyServerRpc(clientId, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.CompareTag("Player"))
        {
            ulong clientId = collision.GetComponentInParent<NetworkObject>().OwnerClientId;
            SetStickyServerRpc(clientId, false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStickyServerRpc(ulong clientId, bool isSticky)
    {
        NetworkObject playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        if (playerObj == null) return;

        RagdollController rc = playerObj.GetComponent<RagdollController>();
        if (rc == null) return;

        if (isSticky)
        {
            rc.moveSpeed = originalSpeed * 0.05f;
            rc.jumpForce = 0f;
            Debug.Log("Player is Sticky (server)");
        }
        else
        {
            rc.moveSpeed = originalSpeed;
            rc.jumpForce = originalJumpForce;
            Debug.Log("Player left Sticky (server)");
        }
    }
}