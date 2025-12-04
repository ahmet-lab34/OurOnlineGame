using UnityEngine;

public class RollingSpike : MonoBehaviour
{
    public float rollSpeed = 5f;

    void Update()
    {
        transform.Rotate(Vector3.forward * rollSpeed * Time.deltaTime);  // Roll
        transform.Translate(Vector3.right * rollSpeed * Time.deltaTime);  // Move right
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Damage or reset player (e.g., respawn)
            other.gameObject.transform.position = Vector3.zero;  // Simple reset
        }
    }
}