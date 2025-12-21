using UnityEngine;

public class PlayerLavaDeath : MonoBehaviour
{
    [SerializeField] private bool instantDeath = true;

    private PlayerScript player;
    private PlayerUIScript ui;

    private void Awake()
    {
        player = GetComponent<PlayerScript>();
        ui = GetComponent<PlayerUIScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Lava")) return;

        if (instantDeath)
        {
            if (ui != null) ui.Die();
        }
        else
        {
            if (player != null) player.GetHit();
        }
    }
}
