using UnityEngine;
using System;

// Define weakPointType enum used by weakPoint
public enum weakPointType
{
    Red,
    Green
}

public class weakPoint : MonoBehaviour
{
    [Header("Weak Point")]
    public weakPointType weakPointType;
    [SerializeField] private int hitsRequired = 1;
    [SerializeField] private int damageToBoss = 10;

    private int currentHits;

    private Health bossHealth;

    public Action<weakPoint> OnDestroyed;

    private void Awake()
    {
        bossHealth = GetComponentInParent<Health>();

        if (bossHealth == null)
        {
            Debug.LogWarning(gameObject.name + ": Boss Health NOT FOUND");
        }
        else
        {
            Debug.Log(gameObject.name + ": Boss Health found. Current HP: " + bossHealth.CurrentHealth);
        }
    }

    public void Hit()
    {
        currentHits++;

        Debug.Log(
            gameObject.name +
            " hit. Type: " +
            weakPointType +
            " | Hits: " +
            currentHits +
            "/" +
            hitsRequired
        );

        if (bossHealth != null)
        {
            Debug.Log(
                gameObject.name +
                " dealing damage: " +
                damageToBoss +
                " | Boss HP before: " +
                bossHealth.CurrentHealth
            );

            bossHealth.TakeDamage(damageToBoss);

            Debug.Log(
                gameObject.name +
                " | Boss HP after: " +
                bossHealth.CurrentHealth
            );
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": Cannot damage boss because Health is NULL");
        }

        if (currentHits >= hitsRequired)
        {
            Debug.Log($"{weakPointType} weak point destroyed!");
            DestroyWeakPoint();
        }
    }

    private void DestroyWeakPoint()
    {
        OnDestroyed?.Invoke(this);

        Destroy(gameObject);
    }
}