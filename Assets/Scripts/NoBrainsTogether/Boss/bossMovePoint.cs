using UnityEngine;

public class bossMovePoint : MonoBehaviour
{
    [SerializeField] private Transform[] points;

    public Vector2 GetRandomPoint()
    {
        if (points.Length == 0)
        {
            Debug.LogWarning("No arena points assigned!");
            return Vector2.zero;
        }

        int randomIndex = Random.Range(0, points.Length);

        return points[randomIndex].position;
    }
}
