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

    private int currentHits;

    public Action<weakPoint> OnDestroyed;

    public void Hit()
    {
        currentHits++;

        Debug.Log($"{weakPointType} weak point hit!");

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