using PurrNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkController : CarController
{ // INHERITANCE
    [SerializeField] private GameObject _lowerPart;
    [SerializeField] private GameObject _wheelColliders;
    [SerializeField] private float _lowerPartRotation;

    private bool _isLowerPartVisible = true;

    protected override void Move(float motorInput, float steerInput, bool handBrakeInput)
    { // POLYMORPHISM
        MotorTorqueDirection = motorInput;
        SteerAngle = steerInput * MaxSteerAngle;
        IsHandBraking = handBrakeInput;

        if (_isLowerPartVisible)
            HandleMovement();
        else
            HandleLowerPartMovement();
    }

    private IEnumerator HandleLowerPartVisibility()
    { // ABSTRACTION
        _isLowerPartVisible = !_isLowerPartVisible;

        if (_isLowerPartVisible)
        {
            Vector3 offsetY = new Vector3(
                transform.position.x,
                transform.position.y + 1,
                transform.position.z
            );
            transform.position = offsetY;
        }

        _lowerPart.SetActive(_isLowerPartVisible);
        Physics.IgnoreLayerCollision(10, 8, !_isLowerPartVisible);

        yield return null;
    }

    private void HandleLowerPartMovement()
    { // ABSTRACTION
        RotateRigidbodyAroundAxis(-transform.forward, SteerAngle, _lowerPartRotation);
        RotateRigidbodyAroundAxis(transform.right, MotorTorqueDirection, _lowerPartRotation);
    }

    private NetworkIdentity _networkIdentity;
    private bool _initialized = false;

    private void Start()
    {
        _networkIdentity = GetComponent<NetworkIdentity>();
        TryInitialize();
    }

    private void TryInitialize()
    {
        if (_initialized) return;
        if (_networkIdentity == null || !_networkIdentity.isOwner) return;
        if (GameManager.IsGameOver) return;

        _initialized = true;
        InitiateVehicle(transform);
        OnEachWheel(UpdateMesh);
    }

    private void Update()
    {
        if (!_initialized)
        {
            TryInitialize();
            return;
        }
        if (GameManager.Instance.IsSelectedVehicle(carType) && !GameManager.IsGameOver)
            ShowSpeed();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsSelectedVehicle(carType) && !GameManager.IsGameOver && GameManager.IsGameStarted)
        {
            float motor = Input.GetAxis("Vertical");
            float steering = Input.GetAxis("Horizontal");
            bool handBrake = Input.GetButton("Jump");

            Move(motor, steering, handBrake);
        }
    }
}