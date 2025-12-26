using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterStat))]
public class MonsterMove : MonoBehaviour
{
    private NavMeshAgent _agent;
    private MonsterStat _stats;
    private CharacterController _characterController;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<MonsterStat>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _agent.speed = _stats.MoveSpeed.Value;
        _agent.stoppingDistance = _stats.AttackRange.Value;
        _agent.angularSpeed = _stats.AngularSpeed.Value;
    }
    public void MoveTo(Vector3 targetPosition)
    {
        _agent.isStopped = false;
        _agent.SetDestination(targetPosition);
    }

    public void Stop()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _agent.velocity = Vector3.zero;
    }

    public void MoveForward()
    {
        Vector3 destination = transform.position + transform.forward * _stats.MoveSpeed.Value;
        _agent.SetDestination(destination);
    }
}
