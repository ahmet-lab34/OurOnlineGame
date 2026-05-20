using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody2D rb;
    private Vector2 direction;
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(direction.x, direction.y) * speed;
    }

    // =========================
    // PUBLIC METHODS
    // =========================

    /// <summary>
    /// Launch projectile in a direction
    /// </summary>
    public void Launch(Vector2 dir)
    {
        direction = dir.normalized;
    }

    /// <summary>
    /// Set projectile speed
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    /// <summary>
    /// Get projectile speed
    /// </summary>
    public float GetSpeed()
    {
        return speed;
    }

    /// <summary>
    /// Set projectile damage
    /// </summary>
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    /// <summary>
    /// Get projectile damage
    /// </summary>
    public float GetDamage()
    {
        return damage;
    }

    /// <summary>
    /// Stop projectile movement
    /// </summary>
    public void StopProjectile()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<weakPoint>().Hit();
            Debug.Log("Hit Enemy Weak Point!");

            Destroy(gameObject);
        }
    }
}