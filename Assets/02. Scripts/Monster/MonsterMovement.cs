using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterStat))]
public class MonsterMovement : MonoBehaviour
{
    private NavMeshAgent _agent;
    private MonsterStat _stats;

    private float _gravity = -9.81f;
    private float _yVelocity = 0f;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<MonsterStat>();
    }

    private void Start()
    {
        _agent.speed = _stats.MoveSpeed.Value;
        _agent.stoppingDistance = _stats.AttackRange.Value;
        _agent.angularSpeed = _stats.AngularSpeed.Value;
    }
    public void MoveTo(Vector3 targetPosition)
    {
        _agent.SetDestination(targetPosition);
    }

    public void MoveForward()
    {
        Vector3 destination = transform.position + transform.forward * _stats.MoveSpeed.Value;
        _agent.SetDestination(destination);
    }
}
