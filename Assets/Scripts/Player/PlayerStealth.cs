using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Animator Param")]
    [SerializeField] private string crouchBool = "IsCrouching";

    public bool IsHidden { get; private set; }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator == null) return;

        // True when crouch animation/state is active (via Animator bool)
        IsHidden = animator.GetBool(crouchBool);
    }
}
