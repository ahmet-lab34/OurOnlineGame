using UnityEngine;

public class BottleFly : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;

    [Header("Flight Settings")]
    public float speed = 5f;
    public float spin = 400f;
    public float lifeTime = 3f;
    public float searchRadius = 12f;

    private bool inFlight;

    private void Awake()
    {
        // Make sure Rigidbody2D reference is assigned
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void Reset()
    {
        // Auto-assign Rigidbody2D when the script is added
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!inFlight) return;
        if (rb == null) return;

        // Safety check: if something froze rotation, unlock it again
        if (rb.freezeRotation)
            rb.freezeRotation = false;

        // Safety check: remove any unexpected constraints
        if (rb.constraints != RigidbodyConstraints2D.None)
            rb.constraints = RigidbodyConstraints2D.None;

        // If angular velocity was reset somehow, reapply spin
        if (Mathf.Abs(rb.angularVelocity) < 1f)
        {
            // Preserve rotation direction if possible
            float sign = rb.angularVelocity >= 0f ? 1f : -1f;
            rb.angularVelocity = spin * sign;
        }
    }

    public void Kick(Vector2 direction)
    {
        if (inFlight) return;
        if (rb == null) return;
        if (direction.sqrMagnitude < 0.0001f) return;

        inFlight = true;

        // Make the bottle physical only after it gets kicked
        Collider2D col = GetComponent<Collider2D>();

        // IMPORTANT: ignore ONLY the branch collider(s) (BranchMarker on branch object)
        if (col != null)
            IgnoreBranchCollision.Apply(col);

        if (col != null)
            col.isTrigger = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;

        // Enable rotation and remove constraints at kick moment
        rb.freezeRotation = false;
        rb.constraints = RigidbodyConstraints2D.None;

        // Clear previous motion for a clean start
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.linearVelocity = direction.normalized * speed;
        rb.angularVelocity = spin;

        // Destroy bottle after lifetime expires
        Destroy(gameObject, lifeTime);
    }

    public bool KickToNearestEnemy()
    {
        Vector2 dir = GetDirectionToNearestEnemy();
        if (dir.sqrMagnitude < 0.001f)
            return false;

        Kick(dir);
        return true;
    }

    private Vector2 GetDirectionToNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        Transform best = null;
        float bestD = float.MaxValue;

        foreach (var h in hits)
        {
            if (h == null) continue;
            if (!h.CompareTag("Enemy")) continue;

            float d = ((Vector2)h.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (d < bestD)
            {
                bestD = d;
                best = h.transform;
            }
        }

        if (best == null) return Vector2.zero;
        return ((Vector2)best.position - (Vector2)transform.position).normalized;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!inFlight) return;
        if (col.collider == null) return;

        // Ignore bullets (if they have the correct tag)
        if (col.collider.CompareTag("Bullet"))
            return;

        // Enemy hit
        if (col.collider.CompareTag("Enemy"))
        {
            BirdBottleReaction bird =
                col.collider.GetComponentInParent<BirdBottleReaction>();

            if (bird != null)
                bird.OnHitByBottle();

            Destroy(gameObject);
            return;
        }

        // Walls / ground / any other surface
        Destroy(gameObject);
    }
}
