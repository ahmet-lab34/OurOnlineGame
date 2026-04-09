using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RagdollController : NetworkBehaviour
{
    [Header("References")]
    public Rigidbody2D pelvis;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float groundRadius = 0.2f;

    [Header("Balance")]
    public BalanceController2D balanceController;

    [Header("Arms")]
    public Arms leftArm;
    public Arms rightArm;

    [Header("Ragdoll Parts")]
    public List<Rigidbody2D> ragdollParts = new List<Rigidbody2D>();

    // Networked positions & rotations
    private NetworkVariable<Vector2>[] netPositions;
    private NetworkVariable<float>[] netRotations;

    // Server-side input
    private Vector2 moveInput;
    private bool jumpInput;
    private Vector2 leftAimInput;
    private Vector2 rightAimInput;

    // Network sync for pelvis and arms
    public NetworkVariable<Vector2> netPelvisPos = new NetworkVariable<Vector2>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector2> netLeftArmAim = new NetworkVariable<Vector2>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector2> netRightArmAim = new NetworkVariable<Vector2>(writePerm: NetworkVariableWritePermission.Server);

    private SharedPlayerCS sharedData;

    void Awake()
    {
        // Enable continuous collision detection to prevent tunneling
        if (pelvis != null) pelvis.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        foreach (var rb in ragdollParts)
        {
            if (rb != null) rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    public override void OnNetworkSpawn()
    {
        sharedData = GetComponentInParent<SharedPlayerCS>();

        // Remove null entries to prevent crashes
        ragdollParts.RemoveAll(rb => rb == null);

        // Only server simulates physics
        if (pelvis != null) pelvis.simulated = IsServer;
        if (leftArm != null) leftArm.enabled = IsServer;
        if (rightArm != null) rightArm.enabled = IsServer;

        // Initialize network arrays
        netPositions = new NetworkVariable<Vector2>[ragdollParts.Count];
        netRotations = new NetworkVariable<float>[ragdollParts.Count];

        for (int i = 0; i < ragdollParts.Count; i++)
        {
            Rigidbody2D rb = ragdollParts[i];
            rb.simulated = IsServer;
            netPositions[i] = new NetworkVariable<Vector2>(rb.position, writePerm: NetworkVariableWritePermission.Server);
            netRotations[i] = new NetworkVariable<float>(rb.rotation, writePerm: NetworkVariableWritePermission.Server);
        }
    }

    void FixedUpdate()
    {
        if (!IsServer) return;

        HandleMovement();
        HandleBalance();
        HandleArms();

        // Sync pelvis and arms
        if (pelvis != null) netPelvisPos.Value = pelvis.position;
        if (leftArm != null) netLeftArmAim.Value = leftAimInput;
        if (rightArm != null) netRightArmAim.Value = rightAimInput;

        // Sync ragdoll parts safely
        for (int i = 0; i < ragdollParts.Count; i++)
        {
            Rigidbody2D rb = ragdollParts[i];
            if (rb == null || netPositions[i] == null || netRotations[i] == null) continue;

            netPositions[i].Value = rb.position;
            netRotations[i].Value = rb.rotation;
        }
    }

    void HandleMovement()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        Vector2 vel = pelvis.linearVelocity;
        vel.x = moveInput.x * moveSpeed;

        if (jumpInput && grounded)
        {
            vel.y = 0f; // reset vertical velocity before jump
            pelvis.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Clamp downward velocity if grounded to prevent sinking
        if (grounded && vel.y < 0f)
            vel.y = 0f;

        pelvis.linearVelocity = vel;
        jumpInput = false;
    }

    void HandleBalance()
    {
        if (balanceController != null)
            balanceController.SetInput(moveInput.x);
    }

    void HandleArms()
    {
        if (leftArm != null) leftArm.SetAim(leftAimInput);
        if (rightArm != null) rightArm.SetAim(rightAimInput);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendLegsInputServerRpc(Vector2 move, bool jump, ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != sharedData.legsPlayerId.Value) return;
        moveInput = move;
        if (jump) jumpInput = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendLeftArmInputServerRpc(Vector2 aim, ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != sharedData.upperPlayerId.Value) return;
        leftAimInput = aim;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendRightArmInputServerRpc(Vector2 aim, ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != sharedData.upperPlayerId.Value) return;
        rightAimInput = aim;
    }

    void Update()
    {
        if (IsServer) return;

        // Skip interpolation for local host
        if (IsOwner) return;

        // Smooth pelvis
        if (pelvis != null)
        {
            pelvis.position = Vector2.Lerp(pelvis.position, netPelvisPos.Value, 0.15f);

            // Optional: prevent remote pelvis from sinking below server position
            Vector2 pos = pelvis.position;
            if (pos.y < netPelvisPos.Value.y) pos.y = netPelvisPos.Value.y;
            pelvis.position = pos;
        }

        // Smooth arms
        if (leftArm != null) leftArm.SetAim(netLeftArmAim.Value);
        if (rightArm != null) rightArm.SetAim(netRightArmAim.Value);

        // Smooth ragdoll parts safely
        for (int i = 0; i < ragdollParts.Count; i++)
        {
            Rigidbody2D rb = ragdollParts[i];
            if (rb == null || netPositions[i] == null || netRotations[i] == null) continue;

            rb.position = Vector2.Lerp(rb.position, netPositions[i].Value, 0.15f);
            rb.rotation = Mathf.LerpAngle(rb.rotation, netRotations[i].Value, 0.15f);
        }
    }
}