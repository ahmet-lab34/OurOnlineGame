using UnityEngine;

public class Scale : MonoBehaviour {
    public Rigidbody2D boxA;
    public Rigidbody2D boxB;
    private Vector2 lastBoxAPos;
    private Vector2 lastBoxBPos;
    
    void Start() {
        if (boxA == null || boxB == null) {
            Debug.LogError("Scale: BoxA and BoxB Rigidbody references must be assigned in the Inspector!");
            enabled = false;
            return;
        }
        Debug.Log("Scale constraint active: BoxA and BoxB synced");
        lastBoxAPos = boxA.position;
        lastBoxBPos = boxB.position;
    }

    void FixedUpdate() {
        Vector2 moveA = boxA.position - lastBoxAPos;
        Vector2 moveB = boxB.position - lastBoxBPos;

        float totalYMove = moveA.y + moveB.y;
        
        if (Mathf.Abs(totalYMove) > 0.0001f) {
            Vector2 targetA = boxA.position;
            Vector2 targetB = boxB.position;
            
            targetA.y -= totalYMove * 0.5f;
            targetB.y -= totalYMove * 0.5f;
            
            boxA.MovePosition(targetA);
            boxB.MovePosition(targetB);

            lastBoxAPos = targetA;
            lastBoxBPos = targetB;
        } else {
            lastBoxAPos = boxA.position;
            lastBoxBPos = boxB.position;
        }
    }
}