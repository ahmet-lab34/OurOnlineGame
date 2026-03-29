using UnityEngine;

public class GravitySwitcher : MonoBehaviour
{
    private bool hasSwitched = false;

    private void OnTriggerStay2D(Collider2D other) {
        if (hasSwitched) return;
        if (!other.CompareTag("Player")) return;

        PlayerStats playerStats = other.GetComponent<PlayerStats>();

        if (playerStats == null) return;
        hasSwitched = true;
        
        Physics2D.gravity = -Physics2D.gravity;

        // Let the player handle its rotation and jump inversion
        playerStats.ToggleUpsideDown();

        Destroy(gameObject);
    }
}
