using System;
using UnityEngine;

public class GravitySwitcherUpsideDown : MonoBehaviour
{
    public static bool isRotated;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GravityToNormal(other);
        }
    }
    public void GravityToNormal(Collider2D other)
    {
        if (!isRotated) return;

        Debug.Log("Gravity flipped - isRotated is now false !");
        isRotated = false;
        Physics2D.gravity = -Physics2D.gravity;
        other.transform.Rotate(0f, 0f, 180f);
        PlayerScript player = other.GetComponent<PlayerScript>();
        if (player != null)
        {
            player.FlipJumpValues();
            player.IsFacingRight = !player.IsFacingRight;
        }
    }
}
