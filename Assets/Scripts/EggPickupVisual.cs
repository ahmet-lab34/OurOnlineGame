using UnityEngine;

public class EggPickupVisual : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite touchedSprite;

    private SpriteRenderer sr;
    private bool changed;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (changed) return;
        if (!other.CompareTag("Player")) return;

        if (sr != null && touchedSprite != null)
        {
            sr.sprite = touchedSprite;
            changed = true;
        }
    }
}