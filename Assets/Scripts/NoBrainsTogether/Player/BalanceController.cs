using UnityEngine;

public class BalanceController2D : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D pelvis;

    [Header("Balance Settings")]
    public float uprightStrength = 250f;
    public float uprightDamping = 15f;

    [Header("Player Control")]
    public float leanTorque = 200f;
    public float maxLeanAngle = 30f;

    [Header("Fall Settings")]
    public float fallAngle = 60f;
    public bool isGrounded = true;

    [Header("Recovery Assist")]
    public float smallSupportAngle = 10f;
    public float strongSupportAngle = 30f;
    public float recoveryStrength = 150f;
    public float recoveryDamping = 10f;

    [Header("Connected Joints")]
    public HingeJoint2D jointA;
    public HingeJoint2D jointB;

    // 🔴 INPUT COMES FROM SERVER CONTROLLER
    private float input;

    // ✅ Called by RagdollController (SERVER ONLY)
    public void SetInput(float newInput)
    {
        input = newInput;
    }

    void FixedUpdate()
    {
        // ⚠️ IMPORTANT: Only run if physics is active (server)
        if (!pelvis.simulated) return;

        if (!isGrounded) return;

        BalanceUpright();
        PlayerLean();
        CheckFall();
        SupportJoint(jointA);
        SupportJoint(jointB);
    }

    void BalanceUpright()
    {
        float angle = GetNormalizedAngle(pelvis.rotation);

        float proportional = -angle * uprightStrength;
        float derivative = -pelvis.angularVelocity * uprightDamping;

        float torque = proportional + derivative;
        torque = Mathf.Clamp(torque, -1000f, 1000f);

        pelvis.AddTorque(torque * Time.fixedDeltaTime);
    }

    void PlayerLean()
    {
        float angle = GetNormalizedAngle(pelvis.rotation);

        if (Mathf.Abs(angle) < maxLeanAngle)
        {
            pelvis.AddTorque(-input * leanTorque);
        }
    }

    void CheckFall()
    {
        float angle = Mathf.Abs(GetNormalizedAngle(pelvis.rotation));

        if (angle > fallAngle)
        {
            Debug.Log("Fallen!");
        }
    }

    void SupportJoint(HingeJoint2D joint)
    {
        if (joint == null) return;

        float jointAngle = joint.jointAngle;
        float absAngle = Mathf.Abs(jointAngle);

        if (absAngle > smallSupportAngle)
        {
            float t = Mathf.Approximately(smallSupportAngle, strongSupportAngle)
                ? 1f
                : Mathf.InverseLerp(smallSupportAngle, strongSupportAngle, absAngle);

            float assistStrength = recoveryStrength * t;

            float proportional = -jointAngle * assistStrength;
            float derivative = -pelvis.angularVelocity * recoveryDamping;

            float torque = proportional + derivative;
            torque = Mathf.Clamp(torque, -1000f, 1000f);

            pelvis.AddTorque(torque * Time.fixedDeltaTime);
        }
    }

    void OnValidate()
    {
        if (strongSupportAngle < smallSupportAngle)
        {
            strongSupportAngle = smallSupportAngle;
        }
    }

    float GetNormalizedAngle(float angle)
    {
        angle %= 360f;

        if (angle <= -180f)
            angle += 360f;
        else if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}