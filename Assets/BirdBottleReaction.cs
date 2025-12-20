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
    [SerializeField] private Vector3 eggOffset = Vector3.zero; // если надо чуть ниже/выше

    [Header("Destroy Settings")]
    [SerializeField] private float destroyDelay = 1.5f;

    private bool triggered;
    private Vector3 flyVelocity;

    // ВАЖНО: место, где птица СИДЕЛА (запоминаем сразу при попадании)
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

        // 1) Запоминаем позицию "гнезда" ДО того, как птица начнёт улетать
        nestPosition = transform.position;

        // 2) Отключаем коллайдеры
        foreach (var c in GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        // 3) Запускаем анимацию улёта
        if (animator != null)
        {
            if (!string.IsNullOrEmpty(isFlyingAwayBool))
                animator.SetBool(isFlyingAwayBool, true);

            if (!string.IsNullOrEmpty(flyAwayTrigger))
                animator.SetTrigger(flyAwayTrigger);

            if (!string.IsNullOrEmpty(flyAwayStateName))
                animator.Play(flyAwayStateName, 0, 0f);
        }

        // 4) Направление полёта
        float dirX = 1f;
        if (randomLeftRight)
            dirX = Random.value < 0.5f ? -1f : 1f;

        flyVelocity = new Vector3(dirX * flySpeedX, flySpeedY, 0f);

        // 5) Через destroyDelay: создать яйцо В ГНЕЗДЕ и уничтожить птицу
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
        // Спавним яйцо именно там, где птица сидела
        if (eggPrefab != null)
            Instantiate(eggPrefab, nestPosition + eggOffset, Quaternion.identity);

        Destroy(gameObject);
    }
}
