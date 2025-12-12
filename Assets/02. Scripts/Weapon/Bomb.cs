using UnityEngine;
using UnityEngine.Pool;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject _explosionEffectPrefab;

    [SerializeField] private float _konckbackForce = 8f;
    [SerializeField] private float _arrackDamage = 20f;

    private IObjectPool<Bomb> _managedPool;
    private bool _isReleased;

    private void OnEnable()
    {
        _isReleased = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Monster"))
        {
            ReleaseToPool();
            return;
        }

        GameObject effectObject = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
        effectObject.transform.position = transform.position;

        Monster monster = other.gameObject.GetComponent<Monster>();
        if (monster != null)
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            Damage damage = new Damage(_arrackDamage, direction, _konckbackForce);
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