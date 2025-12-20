using UnityEngine;

public class BirdBottleReaction : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string flyAwayTrigger = "FlyAway";
    [SerializeField] private string flyAwayStateName = "FlyBird";

    [Header("Block other transitions")]
    [SerializeField] private string isFlyingAwayBool = "IsFlyingAway";

    [Header("Stop other scripts that override animations")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    [Header("Fly Away Movement")]
    [SerializeField] private bool moveWhileFlyingAway = true;
    [SerializeField] private float flySpeedX = 2.5f;   // скорость влево/вправо
    [SerializeField] private float flySpeedY = 3.5f;   // скорость вверх
    [SerializeField] private bool randomLeftRight = true; // случайно влево/вправо
    [SerializeField] private bool faceMoveDirection = true; // развернуть спрайт по направлению

    [Header("Destroy Settings")]
    [SerializeField] private float destroyDelay = 0.6f;

    private bool triggered;
    private Vector3 flyVelocity; // куда летим

    public void OnHitByBottle()
    {
        if (triggered) return;
        triggered = true;

        // 1) Отключаем поведение птицы (стрельбу/детект/и т.п.)
        if (scriptsToDisable != null)
        {
            for (int i = 0; i < scriptsToDisable.Length; i++)
            {
                if (scriptsToDisable[i] != null)
                    scriptsToDisable[i].enabled = false;
            }
        }

        // 2) Отключаем коллайдеры, чтобы больше ни с чем не сталкивалась
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

        // 4) Готовим направление полёта
        float dirX = 1f;
        if (randomLeftRight)
            dirX = Random.value < 0.5f ? -1f : 1f;

        flyVelocity = new Vector3(dirX * flySpeedX, flySpeedY, 0f);

        // Разворачиваем спрайт, если надо
        if (faceMoveDirection)
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                sr.flipX = flyVelocity.x < 0f;
        }

        // 5) Уничтожаем через задержку (ты ставишь в инспекторе)
        Destroy(gameObject, destroyDelay);
    }

    private void Update()
    {
        if (!triggered) return;
        if (!moveWhileFlyingAway) return;

        // двигаем вверх + влево/вправо, пока "улетает"
        transform.position += flyVelocity * Time.deltaTime;
    }
}