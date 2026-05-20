using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Arrival")]
    [SerializeField] private float stoppingDistance = 0.1f;

    private Vector2 targetPosition;
    private bool isMoving;

    public bool IsMoving => isMoving;

    public Vector2 CurrentTarget => targetPosition;

    private void Update()
    {
        if (!isMoving)
            return;

        Move();
        Debug.DrawLine(transform.position, targetPosition, Color.red);
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        float distance = Vector2.Distance(transform.position, targetPosition);

        if (distance <= stoppingDistance)
        {
            StopMoving();
        }
    }

    public void MoveTo(Vector2 target)
    {
        targetPosition = target;
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public bool HasReachedTarget()
    {
        return Vector2.Distance(transform.position, targetPosition) <= stoppingDistance;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}