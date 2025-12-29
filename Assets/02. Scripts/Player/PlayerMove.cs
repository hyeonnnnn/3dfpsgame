using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
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
    private Animator _animator;
    private PlayerStats _stats;
    private Camera _mainCamera;
    private Transform _cameraTransform;

    // [SerializeField] private MoveIndicator _moveIndicator;
    private NavMeshAgent _navMeshAgent;
    private RaycastHit _hitInfo = new RaycastHit();
    private const float INPUT_THRESHOLD = 0.01f;
    private float _hitDistance = 100f;

    private int _jumpCount = 0;
    private float _yVelocity = 0f;

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();

        _mainCamera = Camera.main;
        _cameraTransform = _mainCamera.transform;
    }

    private void Start()
    {
        _stats.MoveSpeed.SetValue(_stats.WalkSpeed.Value);

        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
    }

    private void Update()
    {
        if (GameManager.Instance.State == EGameState.Ready) return;
        if (GameManager.Instance.State == EGameState.GameOver) return;

        ApplyGravity();
        HandleClickMovement();
        Run();

        Vector3 horizontalVelocity = CalculateHorizontalVelocity();
        Vector3 finalVelocity = horizontalVelocity + Vector3.up * _yVelocity;

        _characterController.Move(finalVelocity * Time.deltaTime);
        _navMeshAgent.nextPosition = transform.position;

        TryJump();
    }

    private Vector3 CalculateHorizontalVelocity()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        bool hasKeyboardInput = Mathf.Abs(moveX) > INPUT_THRESHOLD || Mathf.Abs(moveZ) > INPUT_THRESHOLD;

        if (hasKeyboardInput)
        {
            _navMeshAgent.ResetPath();
            // _moveIndicator.Hide();

            Vector3 direction = new Vector3(moveX, 0, moveZ);
            _animator.SetFloat("Speed", direction.magnitude);
            direction.Normalize();

            direction = _cameraTransform.TransformDirection(direction);
            direction.y = 0f;
            direction.Normalize();

            IsMoving = true;
            return direction * _stats.MoveSpeed.Value;
        }

        if (_navMeshAgent.hasPath)
        {
            IsMoving = true;
            return _navMeshAgent.desiredVelocity;
        }

        IsMoving = false;
        return Vector3.zero;
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded)
        {
            if (_yVelocity < 0f)
            {
                _yVelocity = -1f;
            }
        }
        else
        {
            _yVelocity += _config.Gravity * Time.deltaTime;
        }
    }

    private void HandleClickMovement()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out _hitInfo, _hitDistance))
            {
                _navMeshAgent.destination = _hitInfo.point;
                // _moveIndicator.Show(_hitInfo.point);
            }
        }
    }

    private void TryJump()
    {
        if (_characterController.isGrounded)
        {
            _jumpCount = 0;
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
