using UnityEngine;

public class Bullet : MonoBehaviour

{
    [SerializeField] private Collider2D birdCollider;
    [SerializeField] private Collider2D shitCollider;
    private shitAnimation shitAnimation;
    public float speed = 5f;
    private Vector2 direction;
    void Awake()
    {
        shitCollider = GetComponent<Collider2D>();
        birdCollider = GameObject.Find("AngryBird").GetComponent<Collider2D>();

        shitAnimation = FindFirstObjectByType<shitAnimation>();
    }
    void Start()
    {
        Destroy(gameObject, 10f);
        Physics2D.IgnoreCollision(birdCollider, shitCollider, true);
        shitAnimation.shitFlying();
    }
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                Debug.Log("Hello");
                playerScript.GetHit();
            }
            shitAnimation.shitExploding();

            Destroy(gameObject, 0.3f);
        }
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            shitAnimation.shitExploding();
            Destroy(gameObject, 0.3f);
        }
    }
}