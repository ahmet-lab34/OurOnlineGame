using UnityEngine;

public class BossAttackSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossMovement movement;
    [SerializeField] private BossWeakPointComponent weakPoints;
    [SerializeField] private Transform[] firePoints;

    [Header("Projectiles")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Attack Settings")]
    [SerializeField] private float shootInterval = 0.5f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float slamSpeed = 15f;
    [SerializeField] private float chaseSpeed = 6f;

    private float shootTimer;

    private Transform chaseTarget;
    private bool isAttacking;

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

        Transform chosenPoint = firePoints[Random.Range(0, firePoints.Length)];

        GameObject bullet = Instantiate(bulletPrefab, movement.transform.position, movement.transform.rotation);

        Vector2 direction = (chosenPoint.position - movement.transform.position).normalized;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;

        Destroy(bullet, 5f);

        shootTimer = shootInterval;
    }

    public void StopShoot()
    {
        isAttacking = false;
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