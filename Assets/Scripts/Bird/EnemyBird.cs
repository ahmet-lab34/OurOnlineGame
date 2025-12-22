using UnityEngine;
using System.Collections;

public class EnemyBird : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private birdAnimation birdAnim;

    [SerializeField] private EnemyVision birdVision;

    private Transform playerTransform;

    [SerializeField] private float shootInterval = 2f;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
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
        StartCoroutine(ShootingAnimationCoroutine());

        if (!birdVision.IFoundThePlayer || playerTransform == null)
        {
            Debug.Log("Can't find the player :(");
            return;
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetOwner(gameObject);
        if (bulletScript != null)
        {
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