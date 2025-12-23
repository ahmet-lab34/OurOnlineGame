using UnityEngine;

public class BottleInteractable : MonoBehaviour
{
    public KeyCode kickKey = KeyCode.F;

    [Header("Player Kick Animation")]
    public Animator playerAnimator;           
    public string kickTriggerName = "Kick";   

    private BottleFly bottleFly;
    private bool playerInside;
    private Collider2D triggerCol;

    private void Awake()
    {
       
        bottleFly = GetComponentInParent<BottleFly>();
        triggerCol = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        
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
          
            bool kicked = bottleFly.KickToNearestEnemy();
            if (!kicked) return; 

            
            if (playerAnimator != null)
                playerAnimator.SetTrigger(kickTriggerName);

            
            playerInside = false;
            if (triggerCol != null) triggerCol.enabled = false;
        }
    }
}
