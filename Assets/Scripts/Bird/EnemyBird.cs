using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyVision))]
public class EnemyBird : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootInterval = 2f;

    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerScript playerController;
    private EnemyVision birdVision;
    
    private Animator birdAnim;

    private void Awake()
    {
        birdVision = GetComponentInChildren<EnemyVision>();
        birdAnim = GetComponentInChildren<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerRb = player.GetComponent<Rigidbody2D>();
            playerController = player.GetComponent<PlayerScript>();
        }
    }

    private void Start()
    {
        if (playerTransform != null)
        {
            InvokeRepeating(nameof(Shoot), shootInterval, shootInterval);
        }
    }

    private void Shoot()
    {
        if (playerTransform == null) return;
        if (!birdVision.IFoundThePlayer) return;

        // Use public getter from PlayerScript
        if (playerController != null && playerController.IsCrouching())
            return;

        StartCoroutine(ShootingAnimationCoroutine());

        // Instantiate bullet
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
        birdAnim.SetBool("birdShitting", true);
        yield return new WaitForSeconds(0.8f);
        birdAnim.SetBool("birdShitting", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
