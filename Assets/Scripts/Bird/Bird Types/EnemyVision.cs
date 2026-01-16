using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private birdAnimation birdAnim;
    public bool IFoundThePlayer = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            birdAnim.playerDetected();
            IFoundThePlayer = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            birdAnim.playerDetectedFalse();
            IFoundThePlayer = false;
        }
    }
}
