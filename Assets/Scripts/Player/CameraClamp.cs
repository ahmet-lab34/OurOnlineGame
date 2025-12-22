using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    void LateUpdate()
    {
        // TEST: Camera must stay in center always
        transform.position = new Vector3(0f, 0f, transform.position.z);
    }
}