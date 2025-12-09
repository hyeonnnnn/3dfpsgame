using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _walkSpeed = 1f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;

    [Header("스테미나")]
    [SerializeField] private float _stamina;
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _staminaIncreaseRate = 10f;
    [SerializeField] private float _staminaDecreaseRate = 20f;
    [SerializeField] private Slider _playerStaminaUI;
    private const float Gravity = -9.81f;

    private CharacterController _characterController;
    private float _yVelocity = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _moveSpeed = _walkSpeed;
    }

    private void Update()
    {
        Move();
        SpeedUp();
    }

    private void Move()
    {
        _yVelocity += Gravity * Time.deltaTime;

        float moveX = Input.GetAxis("Horizontal") * _moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * _moveSpeed * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && _characterController.isGrounded)
        {
            _yVelocity = _jumpForce;
        }

        Vector3 direction = new Vector3(moveX, 0, moveZ);
        direction.Normalize();

        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = _yVelocity;

        _characterController.Move(direction * _moveSpeed * Time.deltaTime);
    }

    private void SpeedUp()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _stamina > 0)
        {
            _moveSpeed = _runSpeed;
            _stamina -= _staminaDecreaseRate * Time.deltaTime;
        }
        else
        {
            _moveSpeed = _walkSpeed;
            _stamina += _staminaIncreaseRate * Time.deltaTime;
        }
        _playerStaminaUI.value = _stamina / _maxStamina;
        _stamina = Mathf.Clamp(_stamina, 0f, _maxStamina);
    }
}
