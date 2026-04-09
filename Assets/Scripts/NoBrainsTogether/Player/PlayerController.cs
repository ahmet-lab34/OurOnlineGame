using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private Player_Actions actions;
    private SharedPlayerCS sharedData;
    private RagdollController ragdoll;

    [Header("Role flags")]
    public bool isLowerBody;
    public bool isUpperBody;

    public override void OnNetworkSpawn()
    {
        sharedData = GetComponentInParent<SharedPlayerCS>();
        ragdoll = GetComponentInParent<RagdollController>();

        actions = new Player_Actions();
        if (isLowerBody) actions.Bottom.Enable();
        if (isUpperBody) actions.Upper.Enable();
    }

    void Update()
    {
        if (ragdoll == null || actions == null) return;

        // --- Legs input ---
        if (isLowerBody)
        {
            Vector2 move = actions.Bottom.Move.ReadValue<Vector2>();
            bool jump = actions.Bottom.Jump.triggered;
            ragdoll.SendLegsInputServerRpc(move, jump);
        }

        // --- Arms input ---
        if (isUpperBody)
        {
            Vector2 aimLeft = actions.Upper.Move_LeftArm.ReadValue<Vector2>();
            Vector2 aimRight = actions.Upper.Move_RightArm.ReadValue<Vector2>();

            if (ragdoll.leftArm != null)
                ragdoll.SendLeftArmInputServerRpc(aimLeft);

            if (ragdoll.rightArm != null)
                ragdoll.SendRightArmInputServerRpc(aimRight);
        }
    }
}