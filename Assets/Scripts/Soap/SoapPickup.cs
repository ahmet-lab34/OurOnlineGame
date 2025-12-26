using UnityEngine;

public class SoapPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;
    private bool picked;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (picked) return;
        if (!other.CompareTag("Player")) return;

        PlayerScript player = other.GetComponent<PlayerScript>();
        if (player == null) return;

        picked = true;
        player.Heal(healAmount);
        Destroy(gameObject);
    }
}
