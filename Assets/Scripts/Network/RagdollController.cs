using System;
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

    // Server-side input
    private Vector2 moveInput;
    private bool jumpInput;
    private Vector2 leftAimInput;
    private Vector2 rightAimInput;

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

        for (int i = 0; i < ragdollParts.Count; i++)
        {
            Rigidbody2D rb = ragdollParts[i];
            rb.simulated = IsServer;
        }
    }

    void FixedUpdate()
    {
        if (!IsServer) return;

        HandleMovement();
        HandleBalance();
        HandleArms();
    }

    void HandleMovement()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        Vector2 vel = pelvis.linearVelocity;
        if (Math.Abs(balanceController.GetNormalizedAngle(pelvis.rotation)) < 45f) {
            vel.x = moveInput.x * moveSpeed;
        }

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

    [Rpc(SendTo.Server)]
    public void SendLegsInputServerRpc(Vector2 move, bool jump, ulong clientId)
    {
        if (clientId != sharedData.legsPlayerId.Value) return;
        moveInput = move;
        if (jump) jumpInput = true;
    }

    [Rpc(SendTo.Server)]
    public void SendLeftArmInputServerRpc(Vector2 aim, ulong clientId)
    {
        if (clientId != sharedData.upperPlayerId.Value) return;
        leftAimInput = aim;
    }

    [Rpc(SendTo.Server)]
    public void SendRightArmInputServerRpc(Vector2 aim, ulong clientId)
    {
        if (clientId != sharedData.upperPlayerId.Value) return;
        rightAimInput = aim;
    }

    void Update()
    {
        if (IsServer) return;

        // Skip interpolation for local host
        if (IsOwner) return;
    }
}