using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterStat))]
public class MonsterMove : MonoBehaviour
{
    private NavMeshAgent _agent;
    private MonsterStat _stats;
    private CharacterController _characterController;

    [SerializeField] private Animator _animator;

    private float _gravity = -9.81f;
    private float _yVelocity = 0f;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<MonsterStat>();
        _characterController = GetComponent<CharacterController>();

        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        _agent.speed = _stats.MoveSpeed.Value;
        _agent.stoppingDistance = _stats.AttackRange.Value;
        _agent.angularSpeed = _stats.AngularSpeed.Value;
    }
    public void MoveTo(Vector3 targetPosition)
    {
        _animator.SetTrigger("Run");
        _agent.SetDestination(targetPosition);

    }

    public void MoveForward()
    {
        _animator.SetTrigger("Walk");
        Vector3 destination = transform.position + transform.forward * _stats.MoveSpeed.Value;
        _agent.SetDestination(destination);
    }

    public void ApplyGravity()
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
            _yVelocity += _gravity * Time.deltaTime;
        }
    }
}
