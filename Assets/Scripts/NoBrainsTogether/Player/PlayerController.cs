using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private Player_Actions actions;
    private SharedPlayerCS sharedData;
    private RagdollController ragdoll;

    private ulong clientId;

    // cached inputs (event-driven)
    private Vector2 moveInput;
    private bool jumpInput;
    private Vector2 aimLeftInput;
    private Vector2 aimRightInput;
    private bool holdLeftInput;
    private bool holdRightInput;
    

    // RPC throttling
    private float sendRate = 0.05f; // 20 times per second
    private float timer;

    public override void OnNetworkSpawn()
    {
        Debug.Log("PlayerController spawned on client " + NetworkManager.Singleton.LocalClientId);

        sharedData = GetComponentInParent<SharedPlayerCS>();
        ragdoll = GetComponentInParent<RagdollController>();

        clientId = NetworkManager.Singleton.LocalClientId;

        // Input setup
        actions = new Player_Actions();
        actions.Enable();

        // Assign Move and Jump inputs for legs player
        actions.Bottom.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        actions.Bottom.Move.canceled += ctx => moveInput = Vector2.zero;

        actions.Bottom.Jump.performed += ctx => jumpInput = true;
        actions.Bottom.Jump.canceled += ctx => jumpInput = false;

        // Assign aim inputs for arms
        actions.Upper.Move_LeftArm.performed += ctx => aimLeftInput = ctx.ReadValue<Vector2>();
        actions.Upper.Move_LeftArm.canceled += ctx => aimLeftInput = Vector2.zero;

        actions.Upper.Move_RightArm.performed += ctx => aimRightInput = ctx.ReadValue<Vector2>();
        actions.Upper.Move_RightArm.canceled += ctx => aimRightInput = Vector2.zero;
        
        // Assign hold inputs for climbing
        actions.Upper.Hold_LeftArm.performed += ctx => holdLeftInput = true;
        actions.Upper.Hold_LeftArm.canceled += ctx => holdLeftInput = false;

        actions.Upper.Hold_RightArm.performed += ctx => holdRightInput = true;
        actions.Upper.Hold_RightArm.canceled += ctx => holdRightInput = false;

        StartCoroutine(WaitForRoles());
    }

    public override void OnNetworkDespawn()
    {
        if (actions != null)
        {
            actions.Disable();
            actions = null;
        }
    }

    private IEnumerator WaitForRoles()
    {
        while (sharedData.legsPlayerId.Value == 0 || sharedData.upperPlayerId.Value == 0)
        {
            yield return null;
        }

        Debug.Log("Roles assigned: Legs - " + sharedData.legsPlayerId.Value +
                  ", Upper - " + sharedData.upperPlayerId.Value);
    }

    void Update()
    {
        if (ragdoll == null || sharedData == null) return;

        timer += Time.deltaTime;
        if (timer < sendRate) return;
        timer = 0f;

        // --- Legs player ---
        if (sharedData.IsLegsPlayer(clientId))
        {
            if (ragdoll.moveSpeed != 0 || jumpInput) // only send if there's input to reduce unnecessary RPCs
            {
                ragdoll.SendLegsInputServerRpc(moveInput, jumpInput);
                Debug.Log($"Sent Legs Input: {moveInput}, Jump: {jumpInput}");
            }
            /*// reset one-frame jump so it doesn't spam
            jumpInput = false;*/
        }

        // --- Upper player ---
        if (sharedData.IsUpperPlayer(clientId))
        {
            // Aim inputs
            if (ragdoll.leftArm != null)
            {
                ragdoll.SendLeftArmInputServerRpc(aimLeftInput);
            }
            if (ragdoll.rightArm != null)
            {
                ragdoll.SendRightArmInputServerRpc(aimRightInput);
            }

            // Holding inputs
            if (ragdoll.leftHoldingArm != null)
            {
                ragdoll.SendLeftArmHoldServerRpc(holdLeftInput);
            }
            if (ragdoll.rightHoldingArm != null)
            {
                ragdoll.SendRightArmHoldServerRpc(holdRightInput);
            }
        }
    }
}