using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class SpikeDMG_Script : MonoBehaviour
{
    private GameObject playerObject;
    private PlayerScript playerScript;
    void Awake()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerScript = playerObject.GetComponent<PlayerScript>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerScript.DieFromSpikes();
        }
    }
}
