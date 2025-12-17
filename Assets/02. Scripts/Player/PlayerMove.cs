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
    private PlayerStats _stats;
    private Camera _mainCamera;
    private Transform _cameraTransform;

    private NavMeshAgent _navMeshAgent;
    private RaycastHit _hitInfo = new RaycastHit();
    [SerializeField] private MoveIndicator _moveIndicator;

    private int _jumpCount = 0;
    private float _yVelocity = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<PlayerStats>();
        _mainCamera = Camera.main;
        _cameraTransform = _mainCamera.transform;
    }

    private void Start()
    {
        _stats.MoveSpeed.SetValue(_stats.WalkSpeed.Value);
    }

    private void Update()
    {
        if (GameManager.Instance.State == EGameState.Ready) return;
        if (GameManager.Instance.State == EGameState.GameOver) return;

        HandleKeyboardMovement();
        HandleClickMovement();
        Run();
    }

    private void HandleKeyboardMovement()
    {
        _yVelocity += _config.Gravity * Time.deltaTime;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 키보드로 입력할 때
        if (Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveZ) > 0.01f) 
        {
            _moveIndicator.Hide();

            _navMeshAgent.ResetPath();
            _navMeshAgent.updatePosition = false;

            Vector3 direction = new Vector3(moveX, 0, moveZ);
            direction.Normalize();

            direction = _cameraTransform.transform.TransformDirection(direction);
            direction.y = _yVelocity;

            _characterController.Move(direction * _stats.MoveSpeed.Value * Time.deltaTime);
        }
        // 마우스 클릭으로 이동 중일 때
        else if (_navMeshAgent.hasPath) 
        {
            _navMeshAgent.updatePosition = true;
            _navMeshAgent.nextPosition = transform.position;
        }
        // 정지 상태일 때
        else
        {
            Vector3 gravityMove = new Vector3(0, _yVelocity, 0);
            _characterController.Move(gravityMove * Time.deltaTime);
        }

        TryJump();
    }

    private void HandleClickMovement()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out _hitInfo))
            {
                _navMeshAgent.destination = _hitInfo.point;
                _moveIndicator.Show(_hitInfo.point);
            }
        }
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
