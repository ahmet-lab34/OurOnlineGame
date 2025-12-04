using UnityEngine;

using System.Collections;

public class Enemy : EnemyVision

{
    public GameObject bulletPrefab;
    public float shootInterval = 2f;
    public Vector2 shootDirection;

    void Start()
    {
        InvokeRepeating(nameof(Shoot), shootInterval, shootInterval);
    }
    void Shoot()
    {
        StartCoroutine(shittingTime());
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!IFoundThePlayer)
        {
            Debug.Log("Can't find the player :(");
            return;
        }
        Vector2 direction = (player.transform.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet Bullet = bullet.GetComponent<Bullet>();
        if (Bullet != null)
        {
            Bullet.SetDirection(direction);
        }
    }

    private IEnumerator shittingTime()
    {
        birdAnimations.birdShitting();

        yield return new WaitForSeconds(.8f);

        birdAnimations.birdShittingFalse();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}