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

        List<int> usedIndexes = new();

        SpawnType(redWeakPointPrefab, redCount, usedIndexes);
        SpawnType(greenWeakPointPrefab, greenCount, usedIndexes);
    }

    private void SpawnType(weakPoint prefab, int count, List<int> usedIndexes)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Weak point prefab is missing!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No weak point spawn points assigned!");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            if (usedIndexes.Count >= spawnPoints.Length)
            {
                Debug.LogWarning("Not enough spawn points for all weak points!");
                return;
            }

            int randomIndex;

            do
            {
                randomIndex = Random.Range(0, spawnPoints.Length);
            }
            while (usedIndexes.Contains(randomIndex));

            usedIndexes.Add(randomIndex);

            weakPoint newWeakPoint = Instantiate(
                prefab,
                spawnPoints[randomIndex].position,
                Quaternion.identity,
                transform
            );

            Debug.Log(
                "Spawned weak point: " +
                newWeakPoint.weakPointType +
                " at " +
                spawnPoints[randomIndex].name
            );

            newWeakPoint.OnDestroyed += HandleWeakPointDestroyed;

            activeWeakPoints.Add(newWeakPoint);
        }
    }

    private void HandleWeakPointDestroyed(weakPoint weakPoint)
    {
        activeWeakPoints.Remove(weakPoint);

        Debug.Log("Weak point destroyed: " + weakPoint.weakPointType);

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