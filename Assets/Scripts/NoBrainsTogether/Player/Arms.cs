using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(HingeJoint2D))]
public class Arms : NetworkBehaviour
{
    [Header("References")]
    public HingeJoint2D hingeJoint2D;

    [Header("Motor Settings")]
    public float maxSpeed = 600f;
    public float torque = 2000f;

    [Header("Soft Limit Settings")]
    public float softZone = 50f;
    public float limitReturnStrength = 800f;

    [Header("Break Settings")]
    public float breakDuration = 10f;
    private bool isBroken = false;
    private float breakTimer = 0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip breakSound;

    private Vector2 targetAim; // <-- externally set target
    private Vector2 currentAim; // <-- smoothed aim

    void Awake()
    {
        if (hingeJoint2D == null)
        {
            hingeJoint2D = GetComponent<HingeJoint2D>();
            Debug.Log("[Arms] HingeJoint2D auto-assigned in Awake.");
        }
    }

    void FixedUpdate()
    {
        if (!IsServer)
        {
            return;
        }

        if (isBroken)
        {
            Debug.Log("[Arms] Arm is broken — running UpdateBrokenArm.");
            UpdateBrokenArm();
            return;
        }

        // Smooth Aim
        currentAim = Vector2.Lerp(currentAim, targetAim, 0.5f);
        Debug.Log($"[Arms] Current Aim: {currentAim}, Target Aim: {targetAim}");

        ApplyAim(currentAim);
    }

    /// <summary>
    /// Public API to set target aim (world coordinates)
    /// </summary>
    public void SetAim(Vector2 worldPos)
    {
        targetAim = worldPos;
        Debug.Log($"[Arms] SetAim called. New Target: {worldPos}");
    }

    void ApplyAim(Vector2 worldTarget)
    {
        Rigidbody2D rb = hingeJoint2D.attachedRigidbody;
        if (rb == null)
        {
            Debug.LogWarning("[Arms] No attached Rigidbody2D found!");
            return;
        }

        Vector2 pivot = hingeJoint2D.transform.TransformPoint(hingeJoint2D.anchor);
        Vector2 dir = worldTarget - pivot;

        if (dir.sqrMagnitude < 0.0001f)
        {
            Debug.Log("[Arms] Direction too small, skipping ApplyAim.");
            return;
        }

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Compute current angle in the SAME space 
        Vector2 currentDir = hingeJoint2D.transform.right;
        float currentAngle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;

        float delta = Mathf.DeltaAngle(currentAngle, targetAngle);

        float stiffness = 6f; // Adjusted stiffness for better feel
        float damping = 1.5f; // Adjusted damping for better feel

        float angularVelocity = rb.angularVelocity;

        float desiredSpeed = (delta * stiffness) - (angularVelocity * damping);

        Debug.Log($"[Arms] targetAngle={targetAngle}, currentAngle={currentAngle}, delta={delta}, desiredSpeed(before limits)={desiredSpeed}");

        ApplyLimits(ref desiredSpeed);

        desiredSpeed = Mathf.Clamp(desiredSpeed, -maxSpeed, maxSpeed);

        JointMotor2D motor = hingeJoint2D.motor;
        motor.motorSpeed = desiredSpeed;
        motor.maxMotorTorque = torque;

        hingeJoint2D.motor = motor;

        if (!hingeJoint2D.useMotor)
        {
            hingeJoint2D.useMotor = true;
            Debug.Log("[Arms] Motor enabled.");
        }
    }

    void ApplyLimits(ref float desiredSpeed)
    {
        float angle = hingeJoint2D.jointAngle;
        float min = hingeJoint2D.limits.min;
        float max = hingeJoint2D.limits.max;

        if (angle >= min && angle <= max)
        {
            return;
        }

        float overSoft = 0f;
        int pushDir = 0;

        if (angle > max)
        {
            overSoft = angle - max;
            pushDir = -1;
        }
        else if (angle < min)
        {
            overSoft = min - angle;
            pushDir = 1;
        }

        Debug.Log($"[Arms] Out of bounds! angle={angle}, min={min}, max={max}, overSoft={overSoft}");

        if (overSoft >= softZone)
        {
            isBroken = true;
            breakTimer = breakDuration;
            desiredSpeed = 0f;

            Debug.LogWarning("[Arms] ARM BROKEN — exceeded soft zone!");

            if (audioSource && breakSound)
            {
                audioSource.PlayOneShot(breakSound);
                Debug.Log("[Arms] Break sound played.");
            }
        }
        else
        {
            float t = Mathf.Clamp01(overSoft / softZone);
            desiredSpeed += pushDir * t * limitReturnStrength;

            Debug.Log($"[Arms] Soft limit applied. t={t}, adjustedSpeed={desiredSpeed}");
        }
    }

    void UpdateBrokenArm()
    {
        breakTimer -= Time.fixedDeltaTime;

        Debug.Log($"[Arms] Broken timer: {breakTimer}");

        if (breakTimer <= 0f)
        {
            isBroken = false;
            Debug.Log("[Arms] Arm recovered from broken state.");
        }
    }
}