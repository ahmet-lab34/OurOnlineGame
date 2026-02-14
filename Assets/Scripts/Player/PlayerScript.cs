using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerScript : NetworkBehaviour
{
    #region Components
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private Animator animator;
    private GroundCHK groundCheck;
    private AllPlayerAudio audioPlayer;
    #endregion

    #region Movement Settings
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float doubleJumpForce = 7f;
    [SerializeField] private float gravityScale = 1.7f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.3f;
    #endregion

    #region State
    private Vector2 moveInput;
    private bool isFacingRight = true;
    private bool isCrouching;
    private bool canDoubleJump;
    private float dashTimer;
    

    public bool IsCrouching() => isCrouching;

    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();
        groundCheck = GetComponentInChildren<GroundCHK>();
        audioPlayer = GetComponent<AllPlayerAudio>();

        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleDashTimers();
        UpdateAnimations();
        HandleFlip();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (dashTimer > 0)
            rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
    #endregion

    #region Input Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (isCrouching)
        {
            StandUp();
            return;
        }

        if (groundCheck.Grounded)
        {
            Jump(jumpForce);
            canDoubleJump = true;
        }
        else if (canDoubleJump)
        {
            Jump(doubleJumpForce);
            canDoubleJump = false;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        dashTimer = dashDuration;
        // Dash cooldown now handled in PlayerStats
        animator.SetBool("IsDashing", true);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (isCrouching)
            StandUp();
        else if (groundCheck.Grounded)
            Crouch();
    }
    #endregion

    #region Core Mechanics
    private void Jump(float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        audioPlayer?.Jumping();
    }

    private void Crouch()
    {
        isCrouching = true;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.8f, 1);
        animator.SetBool("IsCrouching", true);
    }

    private void StandUp()
    {
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, 1f, 1);
        animator.SetBool("IsCrouching", false);
    }

    private void HandleDashTimers()
    {
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
                animator.SetBool("IsDashing", false);
        }
    }

    private void HandleFlip()
    {
        if (moveInput.x > 0 && !isFacingRight) Flip();
        else if (moveInput.x < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void UpdateAnimations()
    {
        animator.SetBool("IsRunning", Mathf.Abs(moveInput.x) > 0.1f);
        animator.SetBool("IsJumping", !groundCheck.Grounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }
    #endregion
}
