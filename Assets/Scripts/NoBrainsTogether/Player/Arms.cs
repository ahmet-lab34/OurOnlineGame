using System;
using Unity.VisualScripting;
using UnityEngine;

public class Arms : MonoBehaviour
{
    #region Inspector Variables
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private HingeJoint2D hingeJoint2D;

    [Header("Motor Settings")]
    [SerializeField] private float maxSpeed = 600f;
    [SerializeField] private float torque = 2000f;

    [Header("Soft Limit Settings")]
    [SerializeField] private float softZone = 50f;          // distance before hard limit
    [SerializeField] private float limitReturnStrength = 800f;

    [Header("Break Settings")]
    [SerializeField] private float breakDuration = 10f;
    private bool isBroken = false;
    private bool JustRecovered = false;
    private float breakTimer = 0f;

    [Header("Input")]
    [SerializeField] private KeyCode activationButton;

    [Header("Audio")]
    [SerializeField] private AudioClip audioClip;
    private AudioSource audioSource;

    #endregion

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKey(activationButton) && !isBroken) {
            hingeJoint2D.useMotor = true;
            hingeJoint2D.useLimits = false;
            Aim();
        }
        else if (!isBroken) {
            if (JustRecovered) {
                FastRecoverToZero();
            }
            else {
                hingeJoint2D.useMotor = false;
                hingeJoint2D.useLimits = true;
            }
        }
        else {
            BrokenArmUpdate();
            hingeJoint2D.useMotor = false;
            hingeJoint2D.useLimits = false;
        }
    }

    void Aim()
    {
        #region Mouse to World
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 pivot = hingeJoint2D.transform.TransformPoint(hingeJoint2D.anchor);
        Vector2 direction = (Vector2)mouseWorld - pivot;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        float worldAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentWorldAngle = transform.eulerAngles.z;
        float delta = Mathf.DeltaAngle(currentWorldAngle, worldAngle);
        #endregion

        #region PD Controller
        float stiffness = 15f;
        float damping = 0.1f;
        float angularVelocity = hingeJoint2D.attachedRigidbody.angularVelocity;

        float desiredSpeed = (-delta * stiffness) - (angularVelocity * damping);
        #endregion
        //Debug.Log($"Delta: {delta:F2}, Desired Speed: {desiredSpeed:F2}" + $" (Angle: {hingeJoint2D.jointAngle:F2}, Limits: [{hingeJoint2D.limits.min:F2}, {hingeJoint2D.limits.max:F2}])");

        ApplyArmModes(ref desiredSpeed);

        desiredSpeed = Mathf.Clamp(desiredSpeed, -maxSpeed, maxSpeed);

        JointMotor2D motor = hingeJoint2D.motor;
        motor.motorSpeed = desiredSpeed;
        motor.maxMotorTorque = torque;
        hingeJoint2D.motor = motor;
    }
    
    #region Arm Modes
    void ApplyArmModes(ref float desiredSpeed)
    {
        float angle = hingeJoint2D.jointAngle;
        float min = hingeJoint2D.limits.min;
        float max = hingeJoint2D.limits.max;

        // MODE 1 — within limits
        if (angle >= min && angle <= max)
        {
            //Debug.Log("Mode 1: Normal movement.");
            return;
        }

        float overSoft = 0f;
        int pushDirection = 0; // +1 = push up, -1 = push down
        if (angle > max)
        {
            // Exceeded max → push back down
            overSoft = angle - max;
            pushDirection = -1;
        }
        else if (angle < min)
        {
            // Exceeded min → push back up
            overSoft = min - angle;
            pushDirection = +1;
        }

        // MODE 4 — breaking
        if (overSoft >= softZone)
        {
            //Debug.Log("Mode 4: Arm breaks!");
            isBroken = true;
            breakTimer = breakDuration;
            desiredSpeed = 0f;

            if (audioSource && audioClip && !audioSource.isPlaying)
                audioSource.PlayOneShot(audioClip);

            return;
        }

        // MODE 2/3 — soft pushback (single unified)
        float t = Mathf.Clamp01(overSoft / softZone);
        float boundaryDelta = t * limitReturnStrength;

        desiredSpeed += pushDirection * boundaryDelta;
        //Debug.Log($"Soft pushback applied: {boundaryDelta}, direction: {pushDirection}");
    }
    private void BrokenArmUpdate() {
        breakTimer -= Time.deltaTime;
        //Debug.Log($"Arm is broken. Time until recovery: {breakTimer:F2} seconds.");
        if (breakTimer <= 0f) {
            isBroken = false;
            JustRecovered = true;
        }
    }
    void FastRecoverToZero() {
        // Only use motor if it's significantly off from 0
        if (Mathf.Abs(hingeJoint2D.jointAngle) > 0.5f) {
            hingeJoint2D.useMotor = true;

            JointMotor2D motor = hingeJoint2D.motor;

            // Hardcoded fast speed toward 0°
            motor.motorSpeed = hingeJoint2D.jointAngle < 0 ? 360f : -360f;
            motor.maxMotorTorque = 100f; // high torque for fast movement

            hingeJoint2D.motor = motor;
            hingeJoint2D.useLimits = false; // disable limits while snapping
        }
        else {
            JustRecovered = false;
        }
    }
    #endregion
}