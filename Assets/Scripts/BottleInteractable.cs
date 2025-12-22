using UnityEngine;

public class BottleInteractable : MonoBehaviour
{
    public KeyCode kickKey = KeyCode.F;

    [Header("Player Kick Animation")]
    public Animator playerAnimator;           // перетащи сюда Animator игрока в инспекторе (лучше вручную)
    public string kickTriggerName = "Kick";   // должно совпадать с Trigger в Animator

    private BottleFly bottleFly;
    private bool playerInside;
    private Collider2D triggerCol;

    private void Awake()
    {
        // BottleFly висит на родителе Bottle
        bottleFly = GetComponentInParent<BottleFly>();
        triggerCol = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // ≈сли Animator не назначен вручную Ч попробуем найти его у игрока автоматически
        if (playerAnimator == null)
            playerAnimator = other.GetComponentInChildren<Animator>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
    }

    private void Update()
    {
        if (!playerInside) return;
        if (bottleFly == null) return;

        if (Input.GetKeyDown(kickKey))
        {
            // 1) ѕинаем “ќЋ№ ќ если есть враг р€дом
            bool kicked = bottleFly.KickToNearestEnemy();
            if (!kicked) return; // врага нет Ч бутылка остаЄтс€ лежать

            // 2) «апускаем анимацию пинка (если Animator найден/назначен)
            if (playerAnimator != null)
                playerAnimator.SetTrigger(kickTriggerName);

            // 3) ”спешно пнули Ч отключаем взаимодействие
            playerInside = false;
            if (triggerCol != null) triggerCol.enabled = false;
        }
    }
}
