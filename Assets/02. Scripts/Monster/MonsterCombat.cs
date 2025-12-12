using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MonsterCombat : MonoBehaviour
{
    public event Action OnHitComplete;
    public event Action OnDeath;

    private MonsterStats _stats;
    private MonsterMovement _movement;
    private Renderer _renderer;

    private Color _hitColor = Color.red;
    private Color _originalColor;
    private Coroutine _currentCoroutine;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _stats = GetComponent<MonsterStats>();
        _movement = GetComponent<MonsterMovement>();
    }

    private void Start()
    {
        _originalColor = _renderer.material.color;
    }

    public bool TryTakeDamage(Damage damage)
    {
        _stats.Health.Decrease(damage.Value);

        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);

        if (_stats.Health.Value > 0f)
        {
            _currentCoroutine = StartCoroutine(Hit_Coroutine(damage.Direction, damage.KnockbackForce));
            return true;
        }
        else
        {
            _currentCoroutine = StartCoroutine(Death_Coroutine());
            return true;
        }
    }

    public void PerformAttack(PlayerController playerController, Vector3 direction)
    {
        if (playerController != null)
        {
            Damage damage = new Damage(_stats.AttackDamage.Value, direction, _stats.KnockbackForce.Value);
            playerController.TakeDamage(damage);
        }
    }

    private IEnumerator Hit_Coroutine(Vector3 direction, float knockbackForce)
    {
        float elapsed = 0f;
        direction.y = 0f;
        direction.Normalize();
        Vector3 knockbackVelocity = direction * knockbackForce;

        _renderer.material.color = _hitColor;

        while (elapsed < _stats.KnockbackDuration.Value)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / _stats.KnockbackDuration.Value;
            Vector3 velocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, progress);

            _movement.ApplyKnockback(velocity);
            yield return null;
        }

        _renderer.material.color = _originalColor;

        yield return new WaitForSeconds(0.1f);
        _currentCoroutine = null;
        OnHitComplete?.Invoke();
    }

    private IEnumerator Death_Coroutine()
    {
        OnDeath?.Invoke();
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
