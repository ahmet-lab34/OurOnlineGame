using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Ragdoll & Physics")]
    public Rigidbody2D rb;                // Root Rigidbody
    public Rigidbody2D[] ragdollParts;    // All child rigidbodies including root
    public Transform playerPos;
    public LayerMask ground;
    public float positionRadius;

    [Header("Movement Settings")]
    public float jumpForce = 10f;
    public float playerSpeed = 5f;

    private bool isOnGround;

    private Player_2_Actions playerActions;
    private SharedPlayerCS sharedData;

    // -----------------------------------
    // Networked position for syncing client
    public NetworkVariable<Vector2> NetworkedPosition = new NetworkVariable<Vector2>(
        writePerm: NetworkVariableWritePermission.Server);

    // -----------------------------------
    public override void OnNetworkSpawn()
    {
        sharedData = GetComponentInParent<SharedPlayerCS>();

        // Only the legs player reads input
        if (IsOwner)
        {
            playerActions = new Player_2_Actions();
            playerActions.Lower.Enable();
        }

        // Server simulates physics, clients just render
        foreach (var part in ragdollParts)
        {
            part.simulated = IsServer; // True on server, false on clients
        }

        rb.simulated = IsServer; // Dynamic on server, kinematic on client
    }

    // -----------------------------------
    void Update()
    {
        if (!IsOwner || playerActions == null) return;

        // Read input
        Vector2 moveInput = playerActions.Lower.Move.ReadValue<Vector2>();
        bool jumpPressed = playerActions.Lower.Jump.triggered;

        // Send input to server
        SendInputServerRpc(moveInput, jumpPressed);

        // -------------------------------
        // Client-side interpolation
        if (!IsServer)
        {
            // Smoothly move the client kinematic Rigidbody toward the server position
            rb.position = Vector2.Lerp(rb.position, NetworkedPosition.Value, 0.15f);
        }
    }

    // -----------------------------------
    [ServerRpc]
    void SendInputServerRpc(Vector2 moveInput, bool jumpPressed, ServerRpcParams rpcParams = default)
    {
        if (rb == null) return;

        // Horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * playerSpeed, rb.linearVelocity.y);

        // Ground check
        isOnGround = Physics2D.OverlapCircle(playerPos.position, positionRadius, ground);

        // Jump
        if (jumpPressed && isOnGround)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Update networked position for clients
        NetworkedPosition.Value = rb.position;
    }

    // -----------------------------------
    void LateUpdate()
    {
        if (IsServer) return; // Only interpolate on clients

        foreach (var part in ragdollParts)
        {
            if (part == rb) continue;

            // Smooth follow: lerp toward the transform of the server (via synced NetworkTransform or NetworkVariable)
            part.transform.position = Vector3.Lerp(part.transform.position, part.transform.position, 0.2f);
            part.transform.rotation = Quaternion.Lerp(part.transform.rotation, part.transform.rotation, 0.2f);
        }
    }
}