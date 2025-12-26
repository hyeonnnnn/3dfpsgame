using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BossStat))]
public class BossMove : MonoBehaviour
{
    private NavMeshAgent _agent;
    private BossStat _stats;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<BossStat>();
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

    public void LookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _stats.AngularSpeed.Value * Time.deltaTime);
        }
    }

    public void Warp(Vector3 position)
    {
        _agent.Warp(position);
    }

    public void SetAgentEnabled(bool enabled)
    {
        _agent.enabled = enabled;
    }
}
