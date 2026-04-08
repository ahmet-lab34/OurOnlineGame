using System;
using UnityEngine;

public class Balance : MonoBehaviour
{
    public float targertRotation;
    public Rigidbody2D rb;
    public float force;

    public void Update()
    {
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targertRotation, force * Time.deltaTime));
    }
}
