using UnityEngine;

public class BirdBottleReaction : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string flyAwayTrigger = "FlyAway";
    [SerializeField] private string flyAwayStateName = "FlyBird";
    [SerializeField] private string isFlyingAwayBool = "IsFlyingAway";

    [Header("Move While Flying Away")]
    [SerializeField] private bool moveWhileFlyingAway = true;
    [SerializeField] private float flySpeedX = 2.5f;
    [SerializeField] private float flySpeedY = 3.5f;
    [SerializeField] private bool randomLeftRight = true;

    [Header("Egg Spawn")]
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private Vector3 eggOffset = Vector3.zero; // use this to move the egg slightly up/down if needed

    [Header("Destroy Settings")]
    [SerializeField] private float destroyDelay = 1.5f;

    private bool triggered;
    private Vector3 flyVelocity;

    // IMPORTANT: position where the bird was sitting (saved immediately on hit)
    private Vector3 nestPosition;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);
    }

    public void OnHitByBottle()
    {
        if (triggered) return;
        triggered = true;

        // 1) Save the nest position BEFORE the bird starts flying away
        nestPosition = transform.position;

        // 2) Disable all colliders so it no longer interacts with anything
        foreach (var c in GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        // 3) Start fly-away animation
        if (animator != null)
        {
            if (!string.IsNullOrEmpty(isFlyingAwayBool))
                animator.SetBool(isFlyingAwayBool, true);

            if (!string.IsNullOrEmpty(flyAwayTrigger))
                animator.SetTrigger(flyAwayTrigger);

            if (!string.IsNullOrEmpty(flyAwayStateName))
                animator.Play(flyAwayStateName, 0, 0f);
        }

        // 4) Choose fly direction
        float dirX = 1f;
        if (randomLeftRight)
            dirX = Random.value < 0.5f ? -1f : 1f;

        flyVelocity = new Vector3(dirX * flySpeedX, flySpeedY, 0f);

        // 5) After destroyDelay: spawn egg at the nest position and destroy the bird
        Invoke(nameof(SpawnEggAndDestroy), destroyDelay);
    }

    private void Update()
    {
        if (!triggered) return;
        if (!moveWhileFlyingAway) return;

        transform.position += flyVelocity * Time.deltaTime;
    }

    private void SpawnEggAndDestroy()
    {
        // Spawn the egg exactly where the bird was sitting
        if (eggPrefab != null)
            Instantiate(eggPrefab, nestPosition + eggOffset, Quaternion.identity);

        Destroy(gameObject);
    }
}