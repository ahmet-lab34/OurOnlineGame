using UnityEngine;

public class WorldRotator : MonoBehaviour
{
    [SerializeField] private GameObject worldParent; // Drag your scene's root here
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float duration = 10f;

    private bool isActive = false;
    private float timer = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            isActive = true;
            timer = duration;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isActive && timer > 0)
        {
            worldParent.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
        }
        else if (isActive)
        {
            isActive = false;
        }
    }
}
