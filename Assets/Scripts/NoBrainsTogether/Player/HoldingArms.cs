using System;
using UnityEngine;

public class HoldingArms : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D handCollider;
    private bool touchingClimable;
    [SerializeField] private Collider2D[] bodyColliders;
    [SerializeField] private KeyCode keyCode;
    void Awake(){
        handCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        foreach(Collider2D col in bodyColliders){
            Physics2D.IgnoreCollision(handCollider, col);
            Debug.Log("Ignoring collision between " + handCollider.name + " and " + col.name);
        }
    }

    void FixedUpdate(){
        if (touchingClimable && Input.GetKey(keyCode)) {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX |
                            RigidbodyConstraints2D.FreezePositionY;
        }
        else {
            rb.constraints = RigidbodyConstraints2D.None;
        }
    }


    void OnTriggerEnter2D(Collider2D collider) {
        if (!collider.CompareTag("Climable")) return;
        if (collider.attachedRigidbody == null) return;
        
        touchingClimable = true;  
    }

    void OnTriggerExit2D(Collider2D collider){
        if (!collider.CompareTag("Climable")) return;

        touchingClimable = false;
    }
}