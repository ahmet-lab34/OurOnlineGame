using UnityEngine;

public class BossDebug : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossBrain brain;
    [SerializeField] private BossHealth health;
    [SerializeField] private BossMovement movement;
    [SerializeField] private BossAttackSystem attackSystem;
    [SerializeField] private BossWeakPointComponent weakPoints;
    [SerializeField] private BossHitbox hitbox;

    [Header("Debug Options")]
    [SerializeField] private bool showLogs = true;
    [SerializeField] private bool showOnGUI = true;

    private string debugText;

    private void Update()
    {
        if (!showLogs) return;

        BuildDebugString();
        LogToConsole();
    }

    private void BuildDebugString()
    {
        debugText = "===== BOSS DEBUG =====\n";

        // STATE
        debugText += $"State: {GetCurrentState()}\n";

        // HEALTH
        debugText += $"HP: {health.CurrentHealth}\n";

        // MOVEMENT
        debugText += $"Moving: {movement.IsMoving}\n";
        debugText += $"Target Pos: {movement.CurrentTarget}\n";

        // ATTACK SYSTEM
        debugText += $"Attacking: {IsAttacking()}\n";

        // WEAK POINTS
        debugText += $"WeakPoints Active: {GetWeakPointCount()}\n";
        debugText += $"All WeakPoints Cleared: {weakPoints.AllWeakPointsDestroyed}\n";

        // HITBOX / STATES FLAGS
        debugText += $"Body Damage Enabled: {GetBodyDamageState()}\n";

        debugText += "======================";
    }

    private string GetCurrentState()
    {
        if (brain == null) return "NULL";

        // uses reflection-safe fallback
        return brain.GetType()
            .GetField("currentState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(brain)?.GetType()?.Name ?? "Unknown";
    }

    private bool IsAttacking()
    {
        if (attackSystem == null) return false;

        return attackSystem.enabled;
    }

    private int GetWeakPointCount()
    {
        if (weakPoints == null) return 0;

        // reflection fallback (since list is private)
        var field = typeof(BossWeakPointComponent)
            .GetField("activeWeakPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field == null) return -1;

        var list = field.GetValue(weakPoints) as System.Collections.IList;

        return list?.Count ?? 0;
    }

    private bool GetBodyDamageState()
    {
        if (hitbox == null) return false;

        var field = typeof(BossHitbox)
            .GetField("canTakeBodyDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field == null) return false;

        return (bool)field.GetValue(hitbox);
    }

    private void LogToConsole()
    {
        Debug.Log(debugText);
    }

    private void OnGUI()
    {
        if (!showOnGUI) return;

        GUI.color = Color.white;
        GUI.Label(new Rect(10, 10, 500, 500), debugText);
    }
}