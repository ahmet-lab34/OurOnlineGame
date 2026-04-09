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

    void Awake()
    {
        if (hingeJoint2D == null)
            hingeJoint2D = GetComponent<HingeJoint2D>();
    }

    void FixedUpdate()
    {
        if (!enabled) return; // Only server controls arms

        if (isBroken)
        {
            UpdateBrokenArm();
            return;
        }

        ApplyAim(targetAim);
    }

    /// <summary>
    /// Public API to set target aim (world coordinates)
    /// </summary>
    public void SetAim(Vector2 worldPos)
    {
        targetAim = worldPos;
    }

    void ApplyAim(Vector2 worldTarget)
    {
        Vector2 pivot = hingeJoint2D.transform.TransformPoint(hingeJoint2D.anchor);
        Vector2 dir = worldTarget - pivot;

        if (dir.sqrMagnitude < 0.0001f) return;

        float worldAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = hingeJoint2D.transform.eulerAngles.z;
        float delta = Mathf.DeltaAngle(currentAngle, worldAngle);

        // PD Controller
        float stiffness = 15f;
        float damping = 0.1f;
        float angularVelocity = hingeJoint2D.attachedRigidbody.angularVelocity;

        float desiredSpeed = (-delta * stiffness) - (angularVelocity * damping);

        // Apply soft limits
        ApplyLimits(ref desiredSpeed);

        desiredSpeed = Mathf.Clamp(desiredSpeed, -maxSpeed, maxSpeed);

        JointMotor2D motor = hingeJoint2D.motor;
        motor.motorSpeed = desiredSpeed;
        motor.maxMotorTorque = torque;
        hingeJoint2D.motor = motor;
        hingeJoint2D.useMotor = true;
    }

    void ApplyLimits(ref float desiredSpeed)
    {
        float angle = hingeJoint2D.jointAngle;
        float min = hingeJoint2D.limits.min;
        float max = hingeJoint2D.limits.max;

        if (angle >= min && angle <= max) return;

        float overSoft = 0f;
        int pushDir = 0;

        if (angle > max) { overSoft = angle - max; pushDir = -1; }
        else if (angle < min) { overSoft = min - angle; pushDir = 1; }

        if (overSoft >= softZone)
        {
            isBroken = true;
            breakTimer = breakDuration;
            desiredSpeed = 0f;

            if (audioSource && breakSound)
                audioSource.PlayOneShot(breakSound);
        }
        else
        {
            float t = Mathf.Clamp01(overSoft / softZone);
            desiredSpeed += pushDir * t * limitReturnStrength;
        }
    }

    void UpdateBrokenArm()
    {
        breakTimer -= Time.fixedDeltaTime;
        if (breakTimer <= 0f)
        {
            isBroken = false;
        }
    }
}