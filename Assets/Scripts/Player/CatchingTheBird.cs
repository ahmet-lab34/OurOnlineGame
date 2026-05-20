using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CatchingTheBird : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private PlayerStats playerStats;

    private GameObject currentEnemy;
    private bool touchingEnemy = false;

    private void Awake()
    {
        if (playerStats == null)
            playerStats = GetComponentInParent<PlayerStats>();

        // Register Input callbacks
        var input = GetComponent<PlayerInput>();
        var catchAction = input.actions["Catch"];
        var rideAction = input.actions["RidingTheBird"];

        catchAction.performed += ctx => TryCatchBird();
        rideAction.performed += ctx => TryRideBird();
    }

    private void TryCatchBird()
    {
        if (touchingEnemy && currentEnemy != null)
        {
            Destroy(currentEnemy);
            playerStats.CarryBird(); // Public method in PlayerStats
        }
    }

    private void TryRideBird()
    {
        if (playerStats.GetBirdCount() > 0)
        {
            playerStats.ReleaseBirds(); // Public method in PlayerStats
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            touchingEnemy = true;
            currentEnemy = other.gameObject;
            Debug.Log("Touching the enemy!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            touchingEnemy = false;
            currentEnemy = null;
            Debug.Log("Left the enemy!");
        }
    }
}
