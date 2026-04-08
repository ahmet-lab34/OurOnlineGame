using UnityEngine;

public class Sticky : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private float originalJumpForce = 0f;
    private float originalSpeed = 0f;
    private void Awake() {
        originalSpeed = playerController.playerSpeed;
        originalJumpForce = playerController.jumpForce;
    }
    void OnTriggerEnter2D(Collider2D collision2D) {
        if (collision2D.gameObject.CompareTag("Player"))
        {
            playerController.playerSpeed = originalSpeed * 0.05f;
            playerController.jumpForce = 0f;
            Debug.Log("Player is Sticky");
        }
        else
        {
            Debug.Log("Player is not Sticky");
        }
    }
    void OnTriggerExit2D(Collider2D collision2D) {
        if (collision2D.gameObject.CompareTag("Player"))
        {
            playerController.playerSpeed = originalSpeed;
            playerController.jumpForce = originalJumpForce;
        }
    }
}
