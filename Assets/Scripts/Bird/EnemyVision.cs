using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] protected static bool IFoundThePlayer = false;
    [SerializeField] protected birdAnimations birdAnimations;

    void Awake()
    {
        birdAnimations = FindFirstObjectByType<birdAnimations>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("The Bird found the player");
            birdAnimations.playerDetected();
            IFoundThePlayer = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("The Bird missed the player");
            birdAnimations.playerDetectedFalse();
            IFoundThePlayer = false;
        }
    }
}
