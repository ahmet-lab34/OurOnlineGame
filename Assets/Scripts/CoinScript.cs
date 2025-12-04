using NUnit.Framework;
using UnityEngine;
using UnityEngine.VFX;

public class CoinScript : MonoBehaviour
{
    [SerializeField] private int Value;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    public bool hasTriggered;
    private CoinCount coinManager;
    private void Start()
    {
        coinManager = CoinCount.instance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            coinManager.ChangeCoins(Value);
            animator.SetBool("Collected", true);
            audioSource.Play();
            float destroyDelay = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, destroyDelay);
        }
    }
}
