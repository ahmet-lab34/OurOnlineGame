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
    [Tooltip("Joint angle (degrees) at which corrective support begins.")]
    public float smallSupportAngle = 10f;  // When assist starts (set in inspector)
    [Tooltip("Joint angle (degrees) at which full support strength is reached.")]
    public float strongSupportAngle = 30f; // When assist is at max (>= smallSupportAngle)
    public float recoveryStrength = 150f;
    public float recoveryDamping = 10f; 

    [Header("Connected Joints")]
    public HingeJoint2D jointA;
    public HingeJoint2D jointB;


    float input;

    void Update()
    {
        input = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
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

        // Clamp torque to prevent physics explosion
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
            // Optional: disable balance here
        }
    }
    void SupportJoint(HingeJoint2D joint)
    {
        if (joint == null) return;
        // jointAngle gives the rotation of the attached body relative to the connected body
        float jointAngle = joint.jointAngle;
        float absAngle = Mathf.Abs(jointAngle);

        if (absAngle > smallSupportAngle)
        {
            float t;
            if (Mathf.Approximately(smallSupportAngle, strongSupportAngle))
            {
                // avoid divide-by-zero; if thresholds are identical treat as full strength
                t = 1f;
            }
            else
            {
                t = Mathf.InverseLerp(smallSupportAngle, strongSupportAngle, absAngle);
            }

            float assistStrength = recoveryStrength * t;

            // apply supporting torque to the pelvis instead of the limb so both joints
            // contribute to pushing the body back toward upright
            float proportional = -jointAngle * assistStrength;
            float derivative = -pelvis.angularVelocity * recoveryDamping;
            float torque = proportional + derivative;
            // clamp to avoid blowing up when multiple joints fire at once
            torque = Mathf.Clamp(torque, -1000f, 1000f);
            pelvis.AddTorque(torque * Time.fixedDeltaTime);
        }
    }

    void OnValidate()
    {
        // make sure support thresholds are sensible in the inspector
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