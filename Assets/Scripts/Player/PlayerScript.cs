using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class PlayerScript : MonoBehaviour
{
    private AllPlayerAudio PlayerAudio;
    private GroundCHK GroundCheck;
    private Rigidbody2D rb;

    public Animator animator;
    private PlayerInput input;
    private InputAction Sprint;
    private InputAction CrouchAction;
    private InputAction jumpAction;

    [SerializeField] private bool IsCrouching = false;
    public bool IsCrouchingPublic => IsCrouching;

    [HideInInspector] public float Horizontal;
    [SerializeField] private bool DoubleJump = true;
    private float dashLength = .5f;
    private float dashCounter;
    [HideInInspector] public float dashCoolcounter;

    public bool IsFacingRight;

    [SerializeField] private PlayerUIScript PlayerUIScript;

    [SerializeField] private bool isTouchingWall = false;
    [SerializeField] private bool nearTheWall = false;
    [SerializeField] private int wallSide;
    private bool LongJumpRotationChecker = true;

    private UnityEngine.Vector2 normalHeight;
    public PlayerStats movingStats = new PlayerStats();
    private PlayerStats originalStats = new PlayerStats();

    public Numerics playerNumbers = new Numerics();
    private float movementDashSpeedBuffer;

    [SerializeField] private ParticleSystem upsideDownGroundSparks;
    [SerializeField] private Transform sparksPivot;

    private bool isUpsideDown;
    private bool sparksPlaying;

    [SerializeField] private float sparksXMin = 1.5f;
    [SerializeField] private float sparksXMax = 3.0f;

    [SerializeField] private float sparksAirSpeedMin = 1.5f;
    [SerializeField] private float sparksAirSpeedMax = 3.0f;

    [SerializeField] private float sparksBlendSpeed = 14f;
    [SerializeField] private float movementDeadZone = 0.15f;

    private Vector2 currentDir;
    private float currentSpeedMin;
    private float currentSpeedMax;

    [Serializable]
    public class PlayerStats
    {
        public float activeMoveSpeed = 5f;
        public float jumpHeight = 10f;

        public float DoubleJumpHeight = 7f;
        public float dashSpeed = 25f;
        public float dashCooldown = 3f;
        public float slideSpeed = 3f;
        public float gravityScale = 1.7f;

        public float wallJumpYAxis;
        public float wallJumpXAxis;
    }

    [Serializable]
    public class Numerics
    {
        public int playerHealth = 3;
        public int BirdCount = 0;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();

        animator = GetComponentInChildren<Animator>();

        PlayerAudio = FindFirstObjectByType<AllPlayerAudio>();
        GroundCheck = FindFirstObjectByType<GroundCHK>();
        PlayerUIScript = FindFirstObjectByType<PlayerUIScript>();

        Sprint = input.actions.FindAction("Sprint");
        CrouchAction = input.actions.FindAction("Crouch");
        jumpAction = input.actions.FindAction("Jump");

        Time.timeScale = 1f;

        currentSpeedMin = sparksXMin;
        currentSpeedMax = sparksXMax;
        currentDir = IsFacingRight ? Vector2.left : Vector2.right;
    }

    void Start()
    {
        normalHeight = transform.localScale;
        originalStats.activeMoveSpeed = movingStats.activeMoveSpeed;
        originalStats.jumpHeight = movingStats.jumpHeight;
        originalStats.DoubleJumpHeight = movingStats.DoubleJumpHeight;
        originalStats.dashSpeed = movingStats.dashSpeed;
        originalStats.dashCooldown = movingStats.dashCooldown;
        originalStats.slideSpeed = movingStats.slideSpeed;
        originalStats.gravityScale = movingStats.gravityScale;
        originalStats.wallJumpYAxis = movingStats.wallJumpYAxis;
        originalStats.wallJumpXAxis = movingStats.wallJumpXAxis;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new UnityEngine.Vector2(Horizontal * movingStats.activeMoveSpeed, rb.linearVelocity.y);
    }

    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        if (isTouchingWall)
        {
            rb.linearVelocity = new UnityEngine.Vector2(Horizontal * movingStats.activeMoveSpeed, -movingStats.slideSpeed);
        }

        PlayerAudio.walkingSound();

        if (nearTheWall && jumpAction.triggered)
        {
            UnityEngine.Vector2 jumpDirection = new UnityEngine.Vector2(rb.linearVelocity.x, movingStats.wallJumpYAxis);
            rb.linearVelocity = jumpDirection;
            nearTheWall = !nearTheWall;
        }

        animator.SetBool("IsRunning", Horizontal > 0 || Horizontal < 0);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        if (GroundCheck.Grounded)
        {
            animator.SetBool("IsJumping", false);
            Debug.Log("Character is Grounded");
            if (jumpAction.ReadValue<float>() == 0)
            {
                DoubleJump = false;
            }
        }

        if (jumpAction.triggered && !IsCrouching)
        {
            if (DoubleJump)
            {
                rb.AddForce(UnityEngine.Vector2.up * movingStats.DoubleJumpHeight, ForceMode2D.Impulse);
                DoubleJump = false;
                PlayerAudio.Jumping();
            }
            if (GroundCheck.Grounded)
            {
                rb.AddForce(UnityEngine.Vector2.up * movingStats.jumpHeight, ForceMode2D.Impulse);
                DoubleJump = true;
                PlayerAudio.Jumping();
            }
        }

        if (!GroundCheck.Grounded)
        {
            animator.SetBool("IsJumping", true);
        }

        if (jumpAction.triggered && (LongJumpRotationChecker ? rb.linearVelocity.y > 0f : rb.linearVelocity.y < 0f))
        {
            rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        if ((IsFacingRight && Horizontal < 0f) || (!IsFacingRight && Horizontal > 0f))
        {
            Flip();
        }

        if (Sprint.triggered && !IsCrouching)
        {
            if (dashCoolcounter <= 0f && dashCounter <= 0f)
            {
                movementDashSpeedBuffer = movingStats.activeMoveSpeed;
                movingStats.activeMoveSpeed = movingStats.dashSpeed;
                dashCounter = dashLength;
                animator.SetBool("IsDashing", true);
            }
        }

        if (dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;
            if (dashCounter <= 0)
            {
                movingStats.activeMoveSpeed = movementDashSpeedBuffer;
                dashCoolcounter = movingStats.dashCooldown;
                animator.SetBool("IsDashing", false);
            }
        }

        if (dashCoolcounter > 0)
        {
            dashCoolcounter -= Time.deltaTime;
        }

        if (jumpAction.triggered && IsCrouching)
        {
            StandUp();
        }

        UpdateUpsideDownSparks();
    }

    private void UpdateUpsideDownSparks()
    {
        if (upsideDownGroundSparks == null || GroundCheck == null || rb == null) return;

        var main = upsideDownGroundSparks.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var v = upsideDownGroundSparks.velocityOverLifetime;
        v.enabled = true;
        v.space = ParticleSystemSimulationSpace.World;

        bool shouldPlay = isUpsideDown;

        bool grounded = GroundCheck.Grounded;

        Vector2 targetDir = currentDir;
        float targetSpeedMin = currentSpeedMin;
        float targetSpeedMax = currentSpeedMax;

        if (grounded)
        {
            targetSpeedMin = sparksXMin;
            targetSpeedMax = sparksXMax;
            targetDir = IsFacingRight ? Vector2.left : Vector2.right;
        }
        else
        {
            targetSpeedMin = sparksAirSpeedMin;
            targetSpeedMax = sparksAirSpeedMax;

            Vector2 vel = rb.linearVelocity;

            if (vel.sqrMagnitude < movementDeadZone * movementDeadZone)
            {
                targetDir = currentDir;
            }
            else
            {
                targetDir = (-vel).normalized;
            }
        }

        float t = 1f - Mathf.Exp(-sparksBlendSpeed * Time.deltaTime);

        currentDir = Vector2.Lerp(currentDir, targetDir, t);
        float mag = currentDir.magnitude;
        if (mag > 0.0001f) currentDir /= mag;

        currentSpeedMin = Mathf.Lerp(currentSpeedMin, targetSpeedMin, t);
        currentSpeedMax = Mathf.Lerp(currentSpeedMax, targetSpeedMax, t);

        float minS = Mathf.Min(currentSpeedMin, currentSpeedMax);
        float maxS = Mathf.Max(currentSpeedMin, currentSpeedMax);

        v.x = new ParticleSystem.MinMaxCurve(currentDir.x * minS, currentDir.x * maxS);
        v.y = new ParticleSystem.MinMaxCurve(currentDir.y * minS, currentDir.y * maxS);
        v.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        if (shouldPlay)
        {
            if (!sparksPlaying)
            {
                upsideDownGroundSparks.Play(true);
                sparksPlaying = true;
            }
        }
        else
        {
            if (sparksPlaying || upsideDownGroundSparks.isPlaying)
            {
                upsideDownGroundSparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                sparksPlaying = false;
            }
        }
    }

    private void ApplySparksFacing()
    {
        if (sparksPivot == null) return;
        sparksPivot.localRotation = Quaternion.Euler(0f, 0f, IsFacingRight ? 0f : 180f);
    }

    public void SetUpsideDown(bool value)
    {
        isUpsideDown = value;

        if (!isUpsideDown)
        {
            if (upsideDownGroundSparks != null)
                upsideDownGroundSparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            sparksPlaying = false;
        }
    }


    public void ToggleUpsideDown()
    {
        isUpsideDown = !isUpsideDown;
    }

    public void FlipJumpValues()
    {
        movingStats.jumpHeight = -movingStats.jumpHeight;
        movingStats.DoubleJumpHeight = -movingStats.DoubleJumpHeight;
        movingStats.wallJumpYAxis = -movingStats.wallJumpYAxis;
        movingStats.slideSpeed = -movingStats.slideSpeed;
        LongJumpRotationChecker = !LongJumpRotationChecker;
    }

    private void OnDestroy()
    {
        if (!LongJumpRotationChecker)
        {
            Physics2D.gravity = -Physics2D.gravity;
            LongJumpRotationChecker = !LongJumpRotationChecker;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            nearTheWall = true;
            DoubleJump = false;
            Debug.Log("I'm near the wall !");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            nearTheWall = false;
            DoubleJump = true;
            Debug.Log("Far from the wall !");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Touching wall !");
            isTouchingWall = true;
            DoubleJump = false;
            animator.SetBool("IsSliding", true);
            if (collision.transform.position.x < transform.position.x)
            {
                wallSide = -1;
            }
            else
            {
                wallSide = 1;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            animator.SetBool("IsSliding", false);
            Debug.Log("Left the wall !");
            isTouchingWall = false;
            DoubleJump = false;
            wallSide = 0;
        }
    }

    private void OnEnable()
    {
        jumpAction.Enable();
        CrouchAction.Enable();
        CrouchAction.performed += OnCrouchPerformed;
    }

    private void OnDisable()
    {
        if (CrouchAction != null)
        {
            CrouchAction.performed -= OnCrouchPerformed;
            CrouchAction.Disable();
        }
        jumpAction.Disable();
    }

    private void OnCrouchPerformed(InputAction.CallbackContext ctx)
    {
        Crouch();
    }

    private void Crouch()
    {
        if (IsCrouching)
        {
            transform.localScale = new UnityEngine.Vector2(transform.localScale.x, normalHeight.y);
            movingStats.activeMoveSpeed = originalStats.activeMoveSpeed;
            IsCrouching = false;
            animator.SetBool("IsCrouching", false);
        }
        else if (!IsCrouching && GroundCheck.Grounded)
        {
            movingStats.activeMoveSpeed = originalStats.activeMoveSpeed * 0.45f;
            transform.localScale = new UnityEngine.Vector2(transform.localScale.x, normalHeight.y * 0.9f);
            IsCrouching = true;
            animator.SetBool("IsCrouching", true);
        }
    }

    private void StandUp()
    {
        transform.localScale = new UnityEngine.Vector2(transform.localScale.x, 1f);
        movingStats.activeMoveSpeed = originalStats.activeMoveSpeed;
        IsCrouching = false;
        animator.SetBool("IsCrouching", false);
    }

    public void GetHit()
    {
        playerNumbers.playerHealth -= 1;
        PlayerAudio.damagedSound();
        if (playerNumbers.playerHealth <= 0)
        {
            PlayerUIScript.Die();
        }
        Debug.Log("Player Got Hit !");
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        playerNumbers.playerHealth += amount;
        Debug.Log("Player healed +" + amount + ". HP now: " + playerNumbers.playerHealth);
    }

    public void DieFromSpikes()
    {
        PlayerUIScript.Die();
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;
        UnityEngine.Vector2 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void CarryingaBird()
    {
        movingStats.activeMoveSpeed *= 0.85f;
        movingStats.jumpHeight *= 0.85f;
        movingStats.dashSpeed *= 0.85f;
        movingStats.dashCooldown *= 1.15f;
        movingStats.slideSpeed *= 0.80f;
        rb.gravityScale *= 0.80f;
        playerNumbers.BirdCount += 1;
        Debug.Log("I have " + playerNumbers.BirdCount + " birds !");
    }

    public void RidingaBird()
    {
        movingStats.activeMoveSpeed = originalStats.activeMoveSpeed;
        movingStats.jumpHeight = originalStats.jumpHeight;
        movingStats.dashSpeed = originalStats.dashSpeed;
        movingStats.dashCooldown = originalStats.dashCooldown;
        movingStats.slideSpeed = originalStats.slideSpeed;
        movingStats.gravityScale = originalStats.gravityScale;

        playerNumbers.BirdCount = 0;
        Debug.Log("I Don't have any birds anymore :(");
    }
}
