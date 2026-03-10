using PurrNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpyController : CarController
{ // INHERITANCE
    [SerializeField] private float _jumpForce = 2000f;

    protected override void HandleMovementOnAir()
    { // POLYMORPHISM
        RotateRigidbodyAroundAxis(-transform.forward, SteerAngle, _airRotation / 2f);
        RotateRigidbodyAroundAxis(transform.right, MotorTorqueDirection, _airRotation);
    }

    private IEnumerator HandleJump()
    { // ABSTRACTION
        _rigidbody.AddForce(
            (transform.up + (transform.forward * MotorTorqueDirection)) * _jumpForce,
            ForceMode.Impulse
        );

        yield return null;
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