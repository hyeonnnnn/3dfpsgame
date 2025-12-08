using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _jumpForce = 5f;
    private const float Gravity = -9.81f;

    private CharacterController _characterController;
    private float _yVelocity = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
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
}
