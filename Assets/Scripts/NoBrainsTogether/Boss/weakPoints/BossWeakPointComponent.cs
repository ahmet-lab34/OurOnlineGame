using UnityEngine;
using System.Collections.Generic;

public class BossWeakPointComponent : MonoBehaviour
{
    [Header("Weak Point Prefabs")]
    [SerializeField] private weakPoint redWeakPointPrefab;
    [SerializeField] private weakPoint greenWeakPointPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    private List<weakPoint> activeWeakPoints = new();

    public bool AllWeakPointsDestroyed =>
        activeWeakPoints.Count == 0;

    public void SpawnWeakPoints(int redCount, int greenCount)
    {
        ClearWeakPoints();

        SpawnType(redWeakPointPrefab, redCount);
        SpawnType(greenWeakPointPrefab, greenCount);
    }

    private void SpawnType(weakPoint prefab, int count)
    {
        List<int> usedIndexes = new();

        for (int i = 0; i < count; i++)
        {
            int randomIndex;

            do
            {
                randomIndex = Random.Range(0, spawnPoints.Length);
            }
            while (usedIndexes.Contains(randomIndex));

            usedIndexes.Add(randomIndex);

            weakPoint weakPoint = Instantiate(
                prefab,
                spawnPoints[randomIndex].position,
                Quaternion.identity,
                transform
            );

            weakPoint.OnDestroyed += HandleWeakPointDestroyed;

            activeWeakPoints.Add(weakPoint);
        }
    }

    private void HandleWeakPointDestroyed(weakPoint weakPoint)
    {
        activeWeakPoints.Remove(weakPoint);

        Debug.Log("Weak point destroyed");

        if (AllWeakPointsDestroyed)
        {
            Debug.Log("All weak points destroyed!");
        }
    }

    public void ClearWeakPoints()
    {
        foreach (weakPoint weakPoint in activeWeakPoints)
        {
            if (weakPoint != null)
            {
                Destroy(weakPoint.gameObject);
            }
        }

        activeWeakPoints.Clear();
    }
}