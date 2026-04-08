using UnityEngine;

public class SpikeDMG_Script : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) return;
        PlayerStats playerHealth = other.GetComponent<PlayerStats>();
        if (playerHealth == null) return;

        playerHealth.GetHit();
    }
}
