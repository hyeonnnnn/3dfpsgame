using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Renderer))]
public class MonsterCombat : MonoBehaviour
{
    private const float HIT_RECOVERY_TIME = 1.38f;

    public event Action OnHitComplete;
    public event Action OnDeath;

    private MonsterStat _stats;
    private NavMeshAgent _agent;

    [SerializeField] private Renderer _renderer;
    [SerializeField] private Animator _animator;

    private Coroutine _hitRecoveryCoroutine;

    private void Awake()
    {
        _stats = GetComponent<MonsterStat>();
        _agent = GetComponent<NavMeshAgent>();

        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }

    public bool TryTakeDamage(Damage damage)
    {
        _stats.Health.Decrease(damage.Value);

        if (_stats.Health.Value > 0f)
        {
            ProcessHit();
            return true;
        }
        else
        {
            ProcessDeath();
            return true;
        }
    }

    private void ProcessHit()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _animator.SetTrigger("Hit");

        if (_hitRecoveryCoroutine != null)
        {
            StopCoroutine(_hitRecoveryCoroutine);
        }
        _hitRecoveryCoroutine = StartCoroutine(HitRecovery_Coroutine());
    }

    private IEnumerator HitRecovery_Coroutine()
    {
        yield return new WaitForSeconds(HIT_RECOVERY_TIME);

        _agent.isStopped = false;
        _hitRecoveryCoroutine = null;
        OnHitComplete?.Invoke();
    }

    private void ProcessDeath()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _animator.SetTrigger("Death");
        OnDeath?.Invoke();
    }

    public void PerformAttack(PlayerController playerController, Vector3 direction)
    {
        if (playerController != null)
        {
            _animator.SetTrigger("Attack");
        }
    }
}
