using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class HoldingArms : NetworkBehaviour
{
    private Rigidbody2D rb;
    private Collider2D handCollider;

    private bool touchingClimbable;
    private bool climbInput;

    /*[SerializeField] private Collider2D[] bodyColliders;*/
    [SerializeField] private InputActionReference climbAction;
    [SerializeField] private SharedPlayerCS sharedData;

    void Awake()
    {
        handCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        /*foreach (Collider2D col in bodyColliders)
        {
            Physics2D.IgnoreCollision(handCollider, col);
        }*/
    }

    public override void OnNetworkSpawn()
    {
        // Only upper player can control arms
        if (NetworkManager.Singleton.LocalClientId == sharedData.upperPlayerId.Value)
        {
            climbAction?.action.Enable();
        }
        else
        {
            climbAction?.action.Disable();
        }
    }

    void Update()
    {
        // Only upper player sends input
        if (NetworkManager.Singleton.LocalClientId != sharedData.upperPlayerId.Value)
            return;

        if (climbAction == null) return;

        bool isClimbing = climbAction.action.IsPressed();

        SendClimbInputServerRpc(isClimbing);
    }

    void FixedUpdate()
    {
        // ONLY server applies physics
        if (!IsServer) return;

        if (touchingClimbable && climbInput)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                             RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.None;
        }
    }

    // =========================================================
    // 🔵 INPUT → SERVER
    // =========================================================

    [ServerRpc(RequireOwnership = false)]
    void SendClimbInputServerRpc(bool isClimbing, ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != sharedData.upperPlayerId.Value)
            return;

        climbInput = isClimbing;
    }

    // =========================================================
    // 🔴 TRIGGERS (SERVER ONLY)
    // =========================================================

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsServer) return;

        if (!collider.CompareTag("Climable")) return;
        if (collider.attachedRigidbody == null) return;

        touchingClimbable = true;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (!IsServer) return;

        if (!collider.CompareTag("Climable")) return;

        touchingClimbable = false;
    }
}