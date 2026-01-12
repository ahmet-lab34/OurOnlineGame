using System;
using UnityEngine;

public class GravitySwitcher : GravitySwitcherUpsideDown
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SwitchGravity(other);
        }
    }
    private void SwitchGravity(Collider2D other)
    {
        if (isRotated) return;

        isRotated = true;

        Physics2D.gravity = -Physics2D.gravity;

        other.transform.Rotate(0f, 0f, 180f);

        PlayerScript player = other.GetComponent<PlayerScript>();
        if (player != null)
        {
            player.FlipJumpValues();
            player.IsFacingRight = !player.IsFacingRight;
            player.ToggleUpsideDown();
        }
    }
}
