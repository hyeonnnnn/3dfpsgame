using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Serializable]
    public class MoveConfig
    {
        public float Gravity = -9.81f;
        public float RunStaminaValue = 10f;
        public float JumpStaminaValue = 10f;
        public int MaxJumpCount = 2;
    }

    public MoveConfig _config;

    private CharacterController _characterController;
    private PlayerStats _stats;
    private Transform _cameraTransform;

    private int _jumpCount = 0;
    private float _yVelocity = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _stats = GetComponent<PlayerStats>();
        _cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        _stats.MoveSpeed.SetValue(_stats.WalkSpeed.Value);
    }

    private void Update()
    {
        Move();
        Run();
    }

    private void Move()
    {
        _yVelocity += _config.Gravity * Time.deltaTime;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(moveX, 0, moveZ);
        direction.Normalize();

        direction = _cameraTransform.transform.TransformDirection(direction);
        direction.y = _yVelocity;

        _characterController.Move(direction * _stats.MoveSpeed.Value * Time.deltaTime);
        TryJump();
    }

    private void TryJump()
    {
        if (_characterController.isGrounded)
        {
            _jumpCount = 0;
            if (_yVelocity < 0) _yVelocity = -1f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _jumpCount < _config.MaxJumpCount)
        {
            _yVelocity = _stats.JumpForce.Value;
            _jumpCount++;

            if (_jumpCount > 1)
            {
                _stats.Stamina.TryConsume(_config.JumpStaminaValue);
            }
        }
    }

    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _stats.Stamina.TryConsume(_config.RunStaminaValue * Time.deltaTime))
        {
            _stats.MoveSpeed.SetValue(_stats.RunSpeed.Value);
        }
    }
}
