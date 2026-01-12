using UnityEngine;

public class AutoDestroyAfterTime : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifetime = 3f;

    [Header("Offsets")]
    [SerializeField] private Vector3 rightOffset = new Vector3(0.6f, 1.5f, 0f);
    [SerializeField] private Vector3 leftOffset = new Vector3(-0.6f, 1.5f, 0f);

    [Header("Click")]
    [SerializeField] private LayerMask bubbleLayerMask; // set to SpeechBubble in Inspector

    private Transform parent;
    private float baseAbsScaleX;
    private float baseScaleY;
    private float baseScaleZ;

    private Collider2D bubbleCollider;
    private Camera cam;

    private void Awake()
    {
        parent = transform.parent;

        baseAbsScaleX = Mathf.Abs(transform.localScale.x);
        baseScaleY = transform.localScale.y;
        baseScaleZ = transform.localScale.z;

        bubbleCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        cam = Camera.main;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (cam == null || bubbleCollider == null) return;

        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

        // Only check colliders on the bubble layer, ignoring CameraBounds etc.
        Collider2D hit = Physics2D.OverlapPoint(mouseWorld, bubbleLayerMask);

        if (hit == bubbleCollider)
            Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (parent == null) return;

        float parentSignX = Mathf.Sign(parent.lossyScale.x);
        if (parentSignX == 0f) parentSignX = 1f;

        transform.localScale = new Vector3(baseAbsScaleX * parentSignX, baseScaleY, baseScaleZ);

        bool facingLeft = parentSignX < 0f;
        transform.localPosition = facingLeft ? leftOffset : rightOffset;
    }
}

