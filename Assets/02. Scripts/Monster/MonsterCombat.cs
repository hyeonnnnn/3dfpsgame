using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Renderer))]
public class MonsterCombat : MonoBehaviour, IDamageable
{
    public event Action OnHitComplete;
    public event Action OnDeath;

    private MonsterStat _stats;
    private MonsterMove _movement;

    [SerializeField] private Renderer _renderer;
    [SerializeField] private Animator _animator;


    [SerializeField] private GameObject _bloodEffectPrefab;

    private NavMeshAgent _agent;
    private List<GameObject> _spawnedEffects = new List<GameObject>();
    private Coroutine _hitRecoveryCoroutine;
    private const float HIT_RECOVERY_TIME = 1.667f;

    private MonsterItemDrop _itemDrop;
    private bool _isDead = false;

    private void Awake()
    {
        _stats = GetComponent<MonsterStat>();
        _movement = GetComponent<MonsterMove>();
        _itemDrop = GetComponent<MonsterItemDrop>();
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    public bool TryTakeDamage(Damage damage)
    {
        _stats.Health.Decrease(damage.Value);
        _bloodEffectPrefab.transform.forward = damage.Normal;

        if (_stats.Health.Value > 0f)
        {
            ProcessHit(damage);
            return true;
        }
        else
        {
            ProcessDeath();
            return true;
        }
    }

    private void ProcessHit(Damage damage)
    {
        _movement.Stop();
        _animator.SetTrigger("Hit");

        GameObject effect = Instantiate(_bloodEffectPrefab, damage.HitPoint, Quaternion.identity, transform);
        _spawnedEffects.Add(effect);

        if (_hitRecoveryCoroutine != null)
        {
            StopCoroutine(_hitRecoveryCoroutine);
        }
        _hitRecoveryCoroutine = StartCoroutine(HitRecovery_Coroutine());
    }

    private IEnumerator HitRecovery_Coroutine()
    {
        _agent.isStopped = true;
        yield return new WaitForSeconds(HIT_RECOVERY_TIME);
        _agent.isStopped = false;

        _hitRecoveryCoroutine = null;
        OnHitComplete?.Invoke();
    }

    private void ProcessDeath()
    {
        if (_isDead == true) return;
        _isDead = true;

        foreach (GameObject effect in _spawnedEffects)
        {
            if (effect != null)
            {
                effect.SetActive(false);
            }
        }

        _movement.Stop();
        _itemDrop.DropItem();
        _animator.SetTrigger("Death");
        _spawnedEffects.Clear();

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
