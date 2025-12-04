using UnityEngine;

public class GroundCHK : MonoBehaviour
{

    [SerializeField] public LayerMask groundLayer;
    [SerializeField] private Transform GroundCheck;
    public bool Grounded = true;
    void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Grounded = true;
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Grounded = false;
        }
    }
}
