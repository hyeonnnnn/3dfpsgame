using UnityEngine;
using UnityEngine.Pool;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject _explosionEffectPrefab;

    [SerializeField] private float _konckbackForce = 8f;
    [SerializeField] private float _attackDamage = 50f;
    [SerializeField] private float _attackRadius = 2f;

    private IObjectPool<Bomb> _managedPool;
    private bool _isReleased;

    private void OnEnable()
    {
        _isReleased = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);

        Vector3 position = transform.position;
        Collider[] colliders = Physics.OverlapSphere(position, _attackRadius, LayerMask.GetMask("Monster"));

        for (int i = 0; i < colliders.Length; i++)
        {
            Monster monster = colliders[i].GetComponent<Monster>();
            if (monster == null) continue;

            Vector3 direction = (colliders[i].transform.position - position).normalized;

            // 거리에 따라 데미지 적용
            float distance = Vector3.Distance(colliders[i].transform.position, position);
            distance = Mathf.Max(distance, 1f);
            float finalDamage = _attackDamage / distance;

            Damage damage = new Damage(finalDamage, direction, _konckbackForce);
            monster.TryTakeDamage(damage);
        }

        ReleaseToPool();
    }

    public void SetManagedPool(IObjectPool<Bomb> pool)
    {
        _managedPool = pool;
    }

    public void ReleaseToPool()
    {
        if (_isReleased) return;
        _isReleased = true;
        _managedPool?.Release(this);
    }
}