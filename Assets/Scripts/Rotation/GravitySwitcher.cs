using UnityEngine;

public class GravitySwitcher : MonoBehaviour
{
    private bool hasSwitched = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (hasSwitched) return;

        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                hasSwitched = true;

                // Flip global gravity
                Physics2D.gravity = -Physics2D.gravity;

                // Let the player handle its rotation and jump inversion
                playerStats.ToggleUpsideDown();

                Destroy(gameObject);
            }
        }
    }
}
