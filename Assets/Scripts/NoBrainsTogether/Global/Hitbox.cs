using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private BossWeakPointComponent weakPoints;

    [Header("Damage Settings")]
    [SerializeField] private int bodyDamage = 1;

    private bool canTakeBodyDamage = true;

    // ---------------------------
    // BODY DAMAGE (main hitbox)
    // ---------------------------
    public void DealBodyDamage(int damage, int playerID)
    {
        if (!canTakeBodyDamage)
            return;

        // Optional: you can restrict by state later via BossBrain
        health.TakeDamage(damage);

        Debug.Log($"Boss took BODY damage: {damage} from Player {playerID}");
    }

    // ---------------------------
    // WEAK POINT DAMAGE ROUTING
    // ---------------------------
    public void HitWeakPoint(weakPoint weakPoint)
    {
        if (weakPoint == null)
            return;

        weakPoint.Hit();
    }

    // ---------------------------
    // STATE CONTROL (called by states)
    // ---------------------------
    public void SetBodyDamageEnabled(bool value)
    {
        canTakeBodyDamage = value;
    }

    // Example helpers for states
    public void EnableInvulnerability()
    {
        canTakeBodyDamage = false;
    }

    public void DisableInvulnerability()
    {
        canTakeBodyDamage = true;
    }
}