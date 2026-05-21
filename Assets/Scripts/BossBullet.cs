using UnityEngine;

public class BossBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBurnEffect burnEffect = collision.GetComponent<PlayerBurnEffect>();

            if (burnEffect != null)
            {
                burnEffect.Burn();
            }

            Destroy(gameObject);
        }
    }
}