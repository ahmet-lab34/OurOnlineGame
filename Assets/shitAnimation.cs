using UnityEngine;

public class shitAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void shitFlying()
    {
        animator.SetBool("shitFlying", true);
    }

    public void shitExploding()
    {
        animator.SetBool("shitExploding", true);
        
    }

}
