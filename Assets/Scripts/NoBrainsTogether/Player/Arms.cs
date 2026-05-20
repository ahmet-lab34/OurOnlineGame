using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System;
using Unity.Transforms;
using UnityEngine.SocialPlatforms;


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

    private Vector2 targetAim;
    private Vector2 currentAim;
    private bool aimInitialized = false;



    void FixedUpdate() {
        if (!IsServer) return;

        if (isBroken) {
            UpdateBrokenArm();
            return;
        }

        // Initialize aim on first frame to prevent snapping
        if (!aimInitialized) {
            currentAim = targetAim;
            aimInitialized = true;
        }

        // Time-based smoothing (stable across framerates)
        currentAim = Vector2.Lerp(currentAim, targetAim, 1f * Time.fixedDeltaTime);

        ApplyAim(targetAim);
    }

    public void SetAim(Vector2 worldPos) {
        targetAim = worldPos;
    }

    void ApplyAim(Vector2 input) {
        Rigidbody2D rb = hingeJoint2D.attachedRigidbody;

        if (rb == null) return;

        Vector2 worldAnchor = hingeJoint2D.transform.TransformPoint(new Vector2(0.8f, 0f));

        Vector2 worldTarget = worldAnchor + input * 1f;

        Debug.DrawLine(worldAnchor, worldTarget, Color.red);

        Vector2 dir = worldTarget - worldAnchor;

        if (dir.sqrMagnitude < 0.0001f) {
            hingeJoint2D.useMotor = false;
            return;
        }

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentWorldAngle = transform.eulerAngles.z;

        float delta = Mathf.DeltaAngle(currentWorldAngle, targetAngle);

        float stiffness = 8f;
        float damping = 0.1f;


        float desiredSpeed = (-delta * stiffness) - (rb.angularVelocity * damping);

        //ApplyLimits(ref desiredSpeed);

        desiredSpeed = Mathf.Clamp(desiredSpeed, -maxSpeed, maxSpeed);

        JointMotor2D motor = hingeJoint2D.motor;
        motor.motorSpeed = desiredSpeed;
        motor.maxMotorTorque = torque;

        hingeJoint2D.motor = motor;
        hingeJoint2D.useMotor = true;
    }

    void ApplyLimits(ref float desiredSpeed)
    {
        // Current arm direction in world space
        Vector2 currentDir = transform.right;
        float currentAngle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;

        float angle = hingeJoint2D.jointAngle;
        float min = hingeJoint2D.limits.min;
        float max = hingeJoint2D.limits.max;

        Debug.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, 0, min) * Vector2.right), Color.green);
        Debug.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, 0, max) * Vector2.right), Color.blue);
        Debug.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, 0, angle) * Vector2.right), Color.red);

        if (angle >= min && angle <= max) return;

        float overSoft = 0f;
        int pushDir = 0;

        if (angle > max) {
            overSoft = angle - max;
            pushDir = -1;
        }
        else if (angle < min) {
            overSoft = min - angle;
            pushDir = 1;
        }

        if (overSoft >= softZone) {
            isBroken = true;
            breakTimer = breakDuration;
            desiredSpeed = 0f;

            // Disable motor for real "limp" behavior
            hingeJoint2D.useMotor = false;

            if (audioSource && breakSound)
            {
                audioSource.PlayOneShot(breakSound);
            }
        }
        else {
            float t = Mathf.Clamp01(overSoft / softZone);

            // Override speed instead of adding (prevents jitter)
            desiredSpeed = pushDir * t * limitReturnStrength;
        }
    }

    void UpdateBrokenArm() {
        breakTimer -= Time.fixedDeltaTime;

        if (breakTimer <= 0f) {
            isBroken = false;

            // Re-enable motor after recovery
            hingeJoint2D.useMotor = true;
        }
    }
    private float Normalize(float currentAngle) {
        float angle = currentAngle % 360f;
        if (angle < -180f) angle += 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}