/*using UnityEngine;

public class EggTouchSprite : MonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private int value = 10; // ������� ����� ��� ����

    [Header("After touch")]
    [SerializeField] private Sprite touchedSprite;
    [SerializeField] private float destroyDelay = 1.2f;

    private Animator anim;
    private SpriteRenderer sr;
    private CoinCount coinManager;
    private bool touched;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        coinManager = CoinCount.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (touched) return;
        if (!other.CompareTag("Player")) return;

        touched = true;

        // 1) ��������� ������
        if (coinManager != null)
            coinManager.ChangeCoins(value);

        // 2) ������������� �������� ��������
        if (anim != null)
            anim.enabled = false;

        // 3) ������ ������
        if (sr != null && touchedSprite != null)
            sr.sprite = touchedSprite;

        // 4) ������� ���� ����� �����
        Destroy(gameObject, destroyDelay);
    }
}*/