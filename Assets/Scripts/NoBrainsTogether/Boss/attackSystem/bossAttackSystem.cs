using UnityEngine;

public class BossAttackSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Movement movement;
    [SerializeField] private BossWeakPointComponent weakPoints;
    [SerializeField] private Transform[] firePoints;

    [Header("Players")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [Header("Projectiles")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Attack Settings")]
    [SerializeField] private float shootInterval = 0.5f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float slamSpeed = 15f;
    [SerializeField] private float chaseSpeed = 6f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fireballShootSound;

    private float shootTimer;

    private Transform chaseTarget;
    private bool isAttacking;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (!isAttacking) return;

        // Only shoot state uses timer
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }
    }

    // ---------------------------
    // SHOOT ATTACK
    // ---------------------------

    public void StartShoot()
    {
        isAttacking = true;
        shootTimer = 0f;
    }

    public void Shoot()
    {
        if (shootTimer > 0) return;

        if (firePoints == null || firePoints.Length == 0)
        {
            Debug.LogWarning("BossAttackSystem: No fire points assigned!");
            return;
        }

        if (bulletPrefab == null)
        {
            Debug.LogWarning("BossAttackSystem: No bullet prefab assigned!");
            return;
        }

        Transform target = GetClosestPlayer();

        if (target == null)
        {
            Debug.LogWarning("BossAttackSystem: No player target assigned!");
            return;
        }

        Transform firePoint = firePoints[Random.Range(0, firePoints.Length)];

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Vector2 direction = ((Vector2)target.position - (Vector2)firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        if (audioSource != null && fireballShootSound != null)
        {
            audioSource.PlayOneShot(fireballShootSound);
        }

        Destroy(bullet, 5f);

        shootTimer = shootInterval;
    }

    public void StopShoot()
    {
        isAttacking = false;
    }

    private Transform GetClosestPlayer()
    {
        if (player1 == null) return player2;
        if (player2 == null) return player1;

        float distanceToPlayer1 = Vector2.Distance(transform.position, player1.position);
        float distanceToPlayer2 = Vector2.Distance(transform.position, player2.position);

        if (distanceToPlayer1 <= distanceToPlayer2)
        {
            return player1;
        }

        return player2;
    }

    // ---------------------------
    // SLAM ATTACK
    // ---------------------------

    public void StartSlam(Vector2 wallPosition)
    {
        isAttacking = true;
        movement.MoveTo(wallPosition);
    }

    public bool IsAtSlamTarget()
    {
        return movement.HasReachedTarget();
    }

    public void ExecuteSlamEffect()
    {
        // add shockwave / damage logic here
        Debug.Log("Boss Slam Hit!");
    }

    public void StopSlam()
    {
        isAttacking = false;
        movement.StopMoving();
    }

    // ---------------------------
    // CHASE ATTACK
    // ---------------------------

    public void StartChase(Transform target)
    {
        isAttacking = true;
        chaseTarget = target;

        weakPoints.SpawnWeakPoints(1, 1);
    }

    public void UpdateChase()
    {
        if (chaseTarget == null) return;

        movement.MoveTo(chaseTarget.position);
    }

    public void StopChase()
    {
        isAttacking = false;
        chaseTarget = null;

        weakPoints.ClearWeakPoints();
    }

    // ---------------------------
    // GLOBAL STOP
    // ---------------------------

    public void StopAllAttacks()
    {
        isAttacking = false;
        chaseTarget = null;

        movement.StopMoving();
        weakPoints.ClearWeakPoints();
    }
}