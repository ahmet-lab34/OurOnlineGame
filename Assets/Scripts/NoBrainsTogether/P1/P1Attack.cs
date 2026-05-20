using UnityEngine;
using UnityEngine.InputSystem;

public class P1Attack : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform fireOrigin;
    [SerializeField] private Projectile projectile;

    [Header("Attack Settings")]
    [SerializeField] private float fireCooldown = 0.25f;
    [SerializeField] private float stickDeadzone = 0.15f;

    private Player_Actions actions;
    private Camera mainCamera;
    private Vector2 aimInput;
    private bool fireRequested;
    private float cooldownTimer;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (fireOrigin == null)
        {
            fireOrigin = transform;
        }

        actions = new Player_Actions();

        actions.UpperSplit.Enable();

        actions.UpperSplit.Move.performed +=
            ctx => aimInput = ctx.ReadValue<Vector2>();

        actions.UpperSplit.Move.canceled +=
            ctx => aimInput = Vector2.zero;

        actions.UpperSplit.Fire.performed +=
            ctx => fireRequested = true;
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer > 0f)
            return;

        Vector2 aimDirection;
        bool hasAim = TryGetAimDirection(out aimDirection);

        Debug.DrawRay(fireOrigin.position, aimDirection * 2f, hasAim ? Color.red : Color.gray);

        if (hasAim && fireRequested)
        {
            FireProjectile(aimDirection);

            fireRequested = false;
            cooldownTimer = fireCooldown;
        }
    }

    private bool TryGetAimDirection(out Vector2 direction)
    {
        direction = Vector2.zero;

        // Controller aiming
        if (aimInput.magnitude >= stickDeadzone)
        {
            direction = aimInput.normalized;
            return true;
        }

        // Mouse aiming
        if (Mouse.current != null && mainCamera != null)
        {
            Vector2 mouseScreen =
                Mouse.current.position.ReadValue();

            Vector3 mouseWorld =
                mainCamera.ScreenToWorldPoint(mouseScreen);

            direction =
                ((Vector2)mouseWorld - (Vector2)fireOrigin.position)
                .normalized;

            if (direction.sqrMagnitude > 0.001f)
            {
                return true;
            }
        }

        return false;
    }

    private void FireProjectile(Vector2 direction) {

        GameObject projectileInstance = Instantiate(projectilePrefab, fireOrigin.position, Quaternion.identity);

        projectileInstance.GetComponent<Projectile>().Launch(direction);
    }

    private void OnDisable() {
        actions.UpperSplit.Disable();
    }
}