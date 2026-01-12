using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Collider2D birdCollider;
    [SerializeField] private Collider2D shitCollider;

    private shitAnimation shitAnimation;
    public float speed = 5f;
    private Vector2 direction;

    private GameObject owner;

    private void Awake()
    {
        shitCollider = GetComponent<Collider2D>();
        shitAnimation = FindFirstObjectByType<shitAnimation>();
    }

    private void Start()
    {
        Destroy(gameObject, 10f);

        // Ignore collision with the bird (owner)
        if (birdCollider != null && shitCollider != null)
            Physics2D.IgnoreCollision(birdCollider, shitCollider, true);

        // Ignore collision with the branch(es)
        if (shitCollider != null)
            IgnoreBranchCollision.Apply(shitCollider);

        if (shitAnimation != null)
            shitAnimation.shitFlying();
    }

    public void SetOwner(GameObject bird)
    {
        owner = bird;

        if (owner != null)
            birdCollider = owner.GetComponent<Collider2D>();

        // IMPORTANT: if owner is assigned after Start(), still ignore owner collision here
        if (birdCollider != null && shitCollider != null)
            Physics2D.IgnoreCollision(birdCollider, shitCollider, true);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                Debug.Log("Hello");
                playerScript.GetHit();
            }

            if (shitAnimation != null)
                shitAnimation.shitExploding();

            Destroy(gameObject, 0.3f);
            return;
        }

        if (!collision.gameObject.CompareTag("Enemy"))
        {
            if (shitAnimation != null)
                shitAnimation.shitExploding();

            Destroy(gameObject, 0.3f);
        }
    }
}
