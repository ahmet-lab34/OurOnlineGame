using UnityEngine;

public class birdAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void playerDetected()
    {
        animator.SetBool("playerDetected", true);

    }
    public void playerDetectedFalse()
    {
        animator.SetBool("playerDetected", false);
    }


    public void birdShitting()
    {
        animator.SetBool("birdShitting", true);
    }
    public void birdShittingFalse()
    {
        animator.SetBool("birdShitting", false);
    }
}
