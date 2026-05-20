using System.Collections;
using UnityEngine;

public class BossDeathSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Movement movement;
    [SerializeField] private BossAttackSystem attackSystem;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Death Animation")]
    [SerializeField] private string deathAnimationStateName = "BossDeath";
    [SerializeField] private AnimationClip deathAnimationClip;

    [Header("Victory UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private float victoryDelayAfterAnimation = 3f;

    private bool deathStarted;

    private void Start()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (health != null)
        {
            health.OnDeath += HandleBossDeath;
        }
        else
        {
            Debug.LogWarning("BossDeathSequence: Health is not assigned!");
        }
    }

    private void HandleBossDeath()
    {
        if (deathStarted) return;

        deathStarted = true;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        Debug.Log("BossDeathSequence: death started");

        if (attackSystem != null)
        {
            attackSystem.StopAllAttacks();
        }

        if (movement != null)
        {
            movement.StopMoving();
        }

        if (animator != null)
        {
            animator.enabled = true;
            animator.speed = 1f;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;

            animator.Play(deathAnimationStateName, 0, 0f);
            animator.Update(0f);

            Debug.Log("BossDeathSequence: playing animation " + deathAnimationStateName);
        }
        else
        {
            Debug.LogWarning("BossDeathSequence: Animator is not assigned!");
        }

        float animationLength = 1f;

        if (deathAnimationClip != null)
        {
            animationLength = deathAnimationClip.length;
            Debug.Log("BossDeathSequence: death animation length = " + animationLength);
        }
        else
        {
            Debug.LogWarning("BossDeathSequence: Death Animation Clip is not assigned!");
        }

        float timer = 0f;

        while (timer < animationLength)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (animator != null)
        {
            animator.enabled = false;
        }

        Debug.Log("BossDeathSequence: boss hidden after death animation");

        yield return new WaitForSecondsRealtime(victoryDelayAfterAnimation);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Debug.Log("BossDeathSequence: victory panel shown");
        }
        else
        {
            Debug.LogWarning("BossDeathSequence: Victory Panel is not assigned!");
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeath -= HandleBossDeath;
        }
    }
}