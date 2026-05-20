using UnityEngine;
using System;
public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    [Header("Invulnerability")]
    [SerializeField] private bool invulnerable = false;

    public int CurrentHealth { get; private set; }

    public bool IsDead => CurrentHealth <= 0;

    // Events
    public Action<int> OnDamageTaken;
    public Action OnDeath;
    public Action OnHealthChanged;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (invulnerable)
            return;

        if (IsDead)
            return;

        CurrentHealth -= amount;

        if (CurrentHealth < 0)
            CurrentHealth = 0;

        Debug.Log($"Boss took {amount} damage. HP: {CurrentHealth}");

        OnDamageTaken?.Invoke(amount);
        OnHealthChanged?.Invoke();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead)
            return;

        CurrentHealth += amount;

        if (CurrentHealth > maxHealth)
            CurrentHealth = maxHealth;

        OnHealthChanged?.Invoke();
    }

    public void SetInvulnerable(bool value)
    {
        invulnerable = value;
    }

    private void Die()
    {
        Debug.Log("Boss Died");

        OnDeath?.Invoke();
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;

        OnHealthChanged?.Invoke();
    }
}