using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class P1Controller : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Player_Actions actions;

    [SerializeField] private float moveSpeed = 5f;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();

        // Assuming you are using the new Input System
        actions = new Player_Actions();
        actions.UpperSplit.Enable();

        actions.UpperSplit.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        actions.UpperSplit.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (moveInput.sqrMagnitude > 0.01f) // Avoid unnecessary calculations when input is near zero
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    void OnDisable() {
        Player_Actions actions = new Player_Actions();
        actions.UpperSplit.Disable();
    }
}