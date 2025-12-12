using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MonsterMovement : MonoBehaviour
{
    private CharacterController _controller;
    private MonsterStats _stats;

    private float _gravity = -9.81f;
    private float _yVelocity = 0f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stats = GetComponent<MonsterStats>();
    }

    public void MoveTo(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        _controller.Move(direction * _stats.MoveSpeed.Value * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void MoveForward()
    {
        _controller.Move(transform.forward * _stats.MoveSpeed.Value * Time.deltaTime);
    }

    public void ApplyGravity()
    {
        if (_controller.isGrounded)
        {
            _yVelocity = -0.5f;
        }
        else
        {
            _yVelocity += _gravity * Time.deltaTime;
        }

        _controller.Move(new Vector3(0, _yVelocity, 0) * Time.deltaTime);
    }

    public void ApplyKnockback(Vector3 velocity)
    {
        if (_controller.isGrounded)
        {
            _yVelocity = -0.5f;
        }
        else
        {
            _yVelocity += _gravity * Time.deltaTime;
        }
        velocity.y = _yVelocity;

        _controller.Move(velocity * Time.deltaTime);
    }
}
