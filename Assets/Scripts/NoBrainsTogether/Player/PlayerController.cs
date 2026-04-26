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

        actions.Bottom.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        actions.Bottom.Move.canceled += ctx => moveInput = Vector2.zero;

        actions.Bottom.Jump.performed += ctx => jumpInput = true;
        actions.Bottom.Jump.canceled += ctx => jumpInput = false;

        actions.Upper.Move_LeftArm.performed += ctx => aimLeftInput = ctx.ReadValue<Vector2>();
        actions.Upper.Move_LeftArm.canceled += ctx => aimLeftInput = Vector2.zero;

        actions.Upper.Move_RightArm.performed += ctx => aimRightInput = ctx.ReadValue<Vector2>();
        actions.Upper.Move_RightArm.canceled += ctx => aimRightInput = Vector2.zero;

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
            ragdoll.SendLegsInputServerRpc(moveInput, jumpInput, clientId);
            Debug.Log($"Sent Legs Input: {moveInput}, Jump: {jumpInput}");

            // reset one-frame jump so it doesn't spam
            jumpInput = false;
        }

        // --- Upper player ---
        if (sharedData.IsUpperPlayer(clientId))
        {
            if (ragdoll.leftArm != null)
            {
                ragdoll.SendLeftArmInputServerRpc(aimLeftInput, clientId);
                Debug.Log($"Sent Left Arm Input: {aimLeftInput}");
            }

            if (ragdoll.rightArm != null)
            {
                ragdoll.SendRightArmInputServerRpc(aimRightInput, clientId);
                Debug.Log($"Sent Right Arm Input: {aimRightInput}");
            }
        }
    }
}