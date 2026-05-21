using System.Collections;
using UnityEngine;

public class PlayerBurnEffect : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite burnedSprite;

    [Header("Movement")]
    [SerializeField] private Movement movement;
    [SerializeField] private float burnedSpeed = 2f;

    [Header("Heavy Effect")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float burnedMass = 5f;
    [SerializeField] private float burnedLinearDamping = 5f;

    [Header("Burn Settings")]
    [SerializeField] private float burnDuration = 5f;

    private SpriteRenderer spriteRenderer;

    private float originalSpeed;
    private float originalMass;
    private float originalLinearDamping;

    private Coroutine burnCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (movement == null)
        {
            movement = GetComponent<Movement>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (movement != null)
        {
            originalSpeed = movement.moveSpeed;
        }

        if (rb != null)
        {
            originalMass = rb.mass;
            originalLinearDamping = rb.linearDamping;
        }
    }

    public void Burn()
    {
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
        }

        burnCoroutine = StartCoroutine(BurnRoutine());
    }

    private IEnumerator BurnRoutine()
    {
        if (spriteRenderer != null && burnedSprite != null)
        {
            spriteRenderer.sprite = burnedSprite;
        }

        if (movement != null)
        {
            movement.SetMoveSpeed(burnedSpeed);
        }

        if (rb != null)
        {
            rb.mass = burnedMass;
            rb.linearDamping = burnedLinearDamping;
        }

        yield return new WaitForSeconds(burnDuration);

        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }

        if (movement != null)
        {
            movement.SetMoveSpeed(originalSpeed);
        }

        if (rb != null)
        {
            rb.mass = originalMass;
            rb.linearDamping = originalLinearDamping;
        }

        burnCoroutine = null;
    }
}