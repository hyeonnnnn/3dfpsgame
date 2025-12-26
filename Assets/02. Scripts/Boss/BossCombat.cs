using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCombat : MonoBehaviour, IDamageable
{
    private const float HIT_RECOVERY_TIME = 0.5f;

    public event Action OnHitComplete;
    public event Action OnDeath;

    private BossStat _stats;
    private BossMove _movement;
    private Animator _animator;

    [SerializeField] private GameObject _bloodEffectPrefab;

    private List<GameObject> _spawnedEffects = new List<GameObject>();
    private Coroutine _hitRecoveryCoroutine;
    private bool _isDead = false;

    private void Awake()
    {
        _stats = GetComponent<BossStat>();
        _movement = GetComponent<BossMove>();
        _animator = GetComponentInChildren<Animator>();
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (_isDead) return false;

        _stats.Health.Decrease(damage.Value);

        if (_bloodEffectPrefab != null)
        {
            _bloodEffectPrefab.transform.forward = damage.Normal;
        }

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

        if (_bloodEffectPrefab != null)
        {
            GameObject effect = Instantiate(_bloodEffectPrefab, damage.HitPoint, Quaternion.identity, transform);
            _spawnedEffects.Add(effect);
        }

        if (_hitRecoveryCoroutine != null)
        {
            StopCoroutine(_hitRecoveryCoroutine);
        }
        _hitRecoveryCoroutine = StartCoroutine(HitRecovery_Coroutine());
    }

    private IEnumerator HitRecovery_Coroutine()
    {
        yield return new WaitForSeconds(HIT_RECOVERY_TIME);

        _hitRecoveryCoroutine = null;
        OnHitComplete?.Invoke();
    }

    private void ProcessDeath()
    {
        if (_isDead) return;
        _isDead = true;

        foreach (GameObject effect in _spawnedEffects)
        {
            if (effect != null)
            {
                effect.SetActive(false);
            }
        }

        _movement.Stop();
        _animator.SetTrigger("Die");
        _spawnedEffects.Clear();

        OnDeath?.Invoke();
    }

    public void PerformAttack()
    {
        _animator.SetTrigger("Attack");
    }
}
