using UnityEngine;

public class goToLocation : MonoBehaviour
{
    private Vector2 targetPos;
    private Vector2 velocity;

    [SerializeField] private float smoothTime = 0.15f;

    void Awake()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        // Smooth movement toward target
        transform.position = Vector2.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        // Debug line
        Debug.DrawLine(transform.position, targetPos, Color.red);
    }

    // Call this from other scripts
    public void SetTarget(Vector2 worldPos)
    {
        targetPos = worldPos;
    }
}