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

    public void Hit(int playerID)
    {
        // Player 1 can only hit RED
        if (playerID == 1 && weakPointType != weakPointType.Red)
            return;

        // Player 2 can only hit GREEN
        if (playerID == 2 && weakPointType != weakPointType.Green)
            return;

        currentHits++;

        Debug.Log($"{weakPointType} weak point hit!");

        if (currentHits >= hitsRequired)
        {
            DestroyWeakPoint();
        }
    }

    private void DestroyWeakPoint()
    {
        OnDestroyed?.Invoke(this);

        Destroy(gameObject);
    }
}