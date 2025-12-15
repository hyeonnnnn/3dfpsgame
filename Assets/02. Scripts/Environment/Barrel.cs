using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Barrel : MonoBehaviour
{
    [SerializeField] private ConsumableStat _health;
    [SerializeField] private ValueStat _explodeDamage;
    [SerializeField] private ParticleSystem _explosionEffect;

    [SerializeField] private float _explodeRadius = 3f;
    
    [SerializeField] private float _explodeForce = 25f;
    [SerializeField] private float _knockbackForce = 10f;

    [SerializeField] private LayerMask _damageLayer;

    private Rigidbody _rigidbody;

    private bool _isDead;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _health.Initialize();
    }

    public void TryTakeDamage(float damage)
    {
        if (_isDead) return;

        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _health.Decrease(damage);
        if (_health.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        Explode();
        BarrelUp();
        StartCoroutine(DestroyAfterDelay_Coroutine());
    }

    private void Explode()
    {
        if (_explosionEffect != null)
        {
            _explosionEffect.transform.position = transform.position;
            _explosionEffect.Play();
        }

        Vector3 position = transform.position;

        // 몬스터 데미지
        Collider[] colliders = Physics.OverlapSphere(position, _explodeRadius, _damageLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<Monster>(out Monster monster))
            {
                monster.TryTakeDamage(new Damage(_explodeDamage.Value, position, _knockbackForce));
            }
            if (colliders[i].TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TryTakeDamage(new Damage(_explodeDamage.Value, position, _knockbackForce));
            }
            if (colliders[i].TryGetComponent<Barrel>(out Barrel barrel))
            {
                barrel.TryTakeDamage(_explodeDamage.Value);
            }
        }
    }

    private void BarrelUp()
    {
        _rigidbody.AddForce(Vector3.up * _explodeForce, ForceMode.Impulse);
        _rigidbody .AddTorque(UnityEngine.Random.insideUnitSphere * _explodeForce, ForceMode.Impulse);
    }

    private IEnumerator DestroyAfterDelay_Coroutine()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
