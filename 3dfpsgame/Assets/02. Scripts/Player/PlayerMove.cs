using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    public class MoveConfig
    {
        public float Gravity = -9.81f;
        public float YVelocity = 0f;
        public float RunStaminaValue = 1f;
        public float JumpStaminaValue = 10f;
        public int MaxJumpCount = 2;
        public int JumpCount = 0;
    }
    private MoveConfig _moveConfig;
    private CharacterController _characterController;
    private PlayerStats _stats;
    private Transform _cameraTransform;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _stats = GetComponent<PlayerStats>();
        _moveConfig = new MoveConfig();
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
        _moveConfig.YVelocity += _moveConfig.Gravity * Time.deltaTime;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(moveX, 0, moveZ);
        direction.Normalize();

        direction = _cameraTransform.transform.TransformDirection(direction);
        direction.y = _moveConfig.YVelocity;

        _characterController.Move(direction * _stats.MoveSpeed.Value * Time.deltaTime);
        TryJump();
    }

    private void TryJump()
    {
        if (_characterController.isGrounded)
        {
            _moveConfig.JumpCount = 0;
            if (_moveConfig.YVelocity < 0) _moveConfig.YVelocity = -1f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _moveConfig.JumpCount < _moveConfig.MaxJumpCount)
        {
            _moveConfig.YVelocity = _stats.JumpForce.Value;
            _moveConfig.JumpCount++;

            if (_moveConfig.JumpCount > 1)
            {
                _stats.Stamina.TryConsume(_moveConfig.JumpStaminaValue);
            }
        }
    }

    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _stats.Stamina.TryConsume(_moveConfig.RunStaminaValue * Time.deltaTime))
        {
            _stats.MoveSpeed.SetValue(_stats.RunSpeed.Value);
        }
        else
        {
            _stats.MoveSpeed.SetValue(_stats.WalkSpeed.Value);
        }
    }
}
