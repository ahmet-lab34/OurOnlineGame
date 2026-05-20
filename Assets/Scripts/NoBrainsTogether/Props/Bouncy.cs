using System;
using UnityEngine;

public class Bouncy : MonoBehaviour
{
    [Tooltip("Base impulse strength applied along the contact normal.")]
    [SerializeField] private float bounceStrength = 10f;

    [Tooltip("How much bounce strength is reduced after each collision (0 = no bounce, 1 = unchanged).")]
    [Range(0f, 1f)]
    [SerializeField] private float bounceDecay = 0.8f;

    [Tooltip("How quickly the bounce strength restores back to full over time.")]
    [SerializeField] private float bounceRestorePerSecond = 0.5f;

    private float currentBounceFactor = 1f;

    void FixedUpdate()
    {
        // Gradually restore bounce strength over time (like energy recovery).
        currentBounceFactor = Mathf.MoveTowards(currentBounceFactor, 1f, bounceRestorePerSecond * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bouncy collided with: " + collision.gameObject.name);
        if (!collision.gameObject.CompareTag("Player"))
            return;

        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        Vector2 normal = Vector2.zero;
        for (int i = 0; i < collision.contactCount; i++)
            normal += collision.GetContact(i).normal;
        normal.Normalize();

        rb.AddForce(normal * bounceStrength * currentBounceFactor, ForceMode2D.Impulse);

        // Reduce bounce for subsequent collisions to simulate energy loss.
        currentBounceFactor = Mathf.Clamp01(currentBounceFactor * bounceDecay);
    }
}
