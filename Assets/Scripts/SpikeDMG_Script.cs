using System;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class SpikeDMG_Script : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerHealth = other.GetComponent<PlayerStats>();
            if (playerHealth != null)
            {
                playerHealth.GetHit();
            }
        }
    }
}
