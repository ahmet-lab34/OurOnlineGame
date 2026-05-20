using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class HoldingArms : NetworkBehaviour
{
    private Rigidbody2D rb;
    private Collider2D handCollider;

    private bool touchingClimbable;
    public bool climbInput { get; private set; }

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
        // Input is now handled externally
    }

    public void SetClimb(bool isClimbing)
    {
        climbInput = isClimbing;
    }

    void FixedUpdate()
    {
        // ONLY server applies physics
        if (!IsServer) return;

        if (touchingClimbable && climbInput)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                             RigidbodyConstraints2D.FreezePositionY;

            Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.yellow);
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.None;
        }
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