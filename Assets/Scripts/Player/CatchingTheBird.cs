using UnityEngine;
using UnityEngine.InputSystem;

public class CatchingTheBird : MonoBehaviour
{
    private PlayerInput input;
    private InputAction Catch;
    private InputAction RidingTheBird;
    [SerializeField] private PlayerScript player;
    [SerializeField] private bool TouchingTheEnemyWithTrigger = false;
    [SerializeField] private GameObject currentEnemy;
    void Start()
    {
        input = GetComponent<PlayerInput>();
        player = GetComponentInParent<PlayerScript>();
        Catch = input.actions.FindAction("Catch");
        RidingTheBird = input.actions.FindAction("RidingTheBird");
        Catch.Enable();
    }
    void Update()
    {
        if (Catch.triggered && TouchingTheEnemyWithTrigger)
        {
            Destroy(currentEnemy);

            player.CarryingaBird();
        }
        if (RidingTheBird.triggered && player.playerNumbers.BirdCount >= 1)
        {
            player.RidingaBird();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TouchingTheEnemyWithTrigger = true;
            currentEnemy = other.gameObject;
            Debug.Log("The Trigger is touching the enemy !");
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TouchingTheEnemyWithTrigger = false;
            currentEnemy = null;
            Debug.Log("The Trigger Left the enemy@@@");
        }
    }
}
