using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public bool IFoundThePlayer = false;
    private Animator birdAnim;

    void Awake() {
        birdAnim = GetComponentInParent<EnemyBird>().GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) return;

        birdAnim.SetBool("playerDetected", true);
        IFoundThePlayer = true;
    }
    void OnTriggerExit2D(Collider2D other) {
        if (!other.CompareTag("Player")) return;

        birdAnim.SetBool("playerDetected", false);
        IFoundThePlayer = false;
    }
}
