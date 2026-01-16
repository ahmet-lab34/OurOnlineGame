using UnityEngine;
using System.Collections;

public class EnemyBird : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private birdAnimation birdAnim;
    [SerializeField] private EnemyVision birdVision;
    [SerializeField] private float shootInterval = 2f;

    private Transform playerTransform;
    private PlayerScript playerScript;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerScript = player.GetComponent<PlayerScript>();
        }
    }

    void Start()
    {
        if (playerTransform != null)
        {
            InvokeRepeating(nameof(Shoot), shootInterval, shootInterval);
        }
    }

    void Shoot()
    {
        if (playerTransform == null) return;

        if (playerScript != null && playerScript.IsCrouchingPublic)
        {
            return;
        }

        if (!birdVision.IFoundThePlayer)
        {
            return;
        }

        StartCoroutine(ShootingAnimationCoroutine());

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetOwner(gameObject);
            bulletScript.SetDirection(direction);
        }
    }

    private IEnumerator ShootingAnimationCoroutine()
    {
        birdAnim.birdShitting();
        yield return new WaitForSeconds(0.8f);
        birdAnim.birdShittingFalse();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
