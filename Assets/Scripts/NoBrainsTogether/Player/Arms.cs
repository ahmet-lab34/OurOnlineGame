using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Arms : NetworkBehaviour
{
    #region Inspector Variables
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private HingeJoint2D hingeJoint2D;

    [Header("Motor Settings")]
    [SerializeField] private float maxSpeed = 600f;
    [SerializeField] private float torque = 2000f;

    [Header("Soft Limit Settings")]
    [SerializeField] private float softZone = 50f;
    [SerializeField] private float limitReturnStrength = 800f;

    [Header("Break Settings")]
    [SerializeField] private float breakDuration = 10f;
    private bool isBroken = false;
    private bool JustRecovered = false;
    private float breakTimer = 0f;

    [Header("Input")]
    [SerializeField] private InputActionReference aimAction;

    [Header("Audio")]
    [SerializeField] private AudioClip audioClip;
    private AudioSource audioSource;

    [SerializeField] private SharedPlayerCS sharedData;
    #endregion

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        audioSource = GetComponent<AudioSource>();
    }

    public override void OnNetworkSpawn()
    {
        // Only enable input for the upper player
        if (NetworkManager.Singleton.LocalClientId == sharedData.upperPlayerId.Value)
        {
            if (aimAction != null)
                aimAction.action.Enable();
        }
        else
        {
            if (aimAction != null)
                aimAction.action.Disable();
        }
    }

    void Update()
    {
        // Only upper player should control the arm
        if (!IsOwner) return;
        if (NetworkManager.Singleton.LocalClientId != sharedData.upperPlayerId.Value) return;

        bool isAiming = aimAction != null && aimAction.action.IsPressed();

        if (isAiming && !isBroken)
        {
            hingeJoint2D.useMotor = true;
            hingeJoint2D.useLimits = false;
            Aim();
        }
        else if (!isBroken)
        {
            if (JustRecovered)
                FastRecoverToZero();
            else
            {
                hingeJoint2D.useMotor = false;
                hingeJoint2D.useLimits = true;
            }
        }
        else
        {
            BrokenArmUpdate();
            hingeJoint2D.useMotor = false;
            hingeJoint2D.useLimits = false;
        }
    }

    #region Arm Movement
    void Aim()
    {
        // Convert mouse to world
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Vector2 pivot = hingeJoint2D.transform.TransformPoint(hingeJoint2D.anchor);
        Vector2 direction = (Vector2)mouseWorld - pivot;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        float worldAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentWorldAngle = transform.eulerAngles.z;
        float delta = Mathf.DeltaAngle(currentWorldAngle, worldAngle);

        // PD controller
        float stiffness = 15f;
        float damping = 0.1f;
        float angularVelocity = hingeJoint2D.attachedRigidbody.angularVelocity;

        float desiredSpeed = (-delta * stiffness) - (angularVelocity * damping);

        ApplyArmModes(ref desiredSpeed);

        desiredSpeed = Mathf.Clamp(desiredSpeed, -maxSpeed, maxSpeed);

        JointMotor2D motor = hingeJoint2D.motor;
        motor.motorSpeed = desiredSpeed;
        motor.maxMotorTorque = torque;
        hingeJoint2D.motor = motor;
    }

    void ApplyArmModes(ref float desiredSpeed)
    {
        float angle = hingeJoint2D.jointAngle;
        float min = hingeJoint2D.limits.min;
        float max = hingeJoint2D.limits.max;

        if (angle >= min && angle <= max)
            return;

        float overSoft = 0f;
        int pushDirection = 0;
        if (angle > max) { overSoft = angle - max; pushDirection = -1; }
        else if (angle < min) { overSoft = min - angle; pushDirection = 1; }

        if (overSoft >= softZone)
        {
            isBroken = true;
            breakTimer = breakDuration;
            desiredSpeed = 0f;

            if (audioSource && audioClip && !audioSource.isPlaying)
                audioSource.PlayOneShot(audioClip);

            return;
        }

        float t = Mathf.Clamp01(overSoft / softZone);
        float boundaryDelta = t * limitReturnStrength;

        desiredSpeed += pushDirection * boundaryDelta;
    }

    private void BrokenArmUpdate()
    {
        breakTimer -= Time.deltaTime;
        if (breakTimer <= 0f)
        {
            isBroken = false;
            JustRecovered = true;
        }
    }

    void FastRecoverToZero()
    {
        if (Mathf.Abs(hingeJoint2D.jointAngle) > 0.5f)
        {
            hingeJoint2D.useMotor = true;

            JointMotor2D motor = hingeJoint2D.motor;
            motor.motorSpeed = hingeJoint2D.jointAngle < 0 ? 360f : -360f;
            motor.maxMotorTorque = 100f;

            hingeJoint2D.motor = motor;
            hingeJoint2D.useLimits = false;
        }
        else
        {
            JustRecovered = false;
        }
    }
    #endregion
}