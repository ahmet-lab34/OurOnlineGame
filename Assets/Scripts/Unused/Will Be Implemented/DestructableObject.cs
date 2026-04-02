/*using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DestructibleBranch : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == null) return;

        // 1️⃣ Check if it is a bullet
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Ignore collision with bullets
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
            return;
        }

        // 2️⃣ Check if it is the player and if dashing
        PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
        if (player != null && player.IsDashing) // assuming IsDashing is a public property
        {
            Destroy(gameObject); // Destroy this branch
        }
    }
}
*/