using UnityEngine;
using UnityEngine.Pool;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject _explosionEffectPrefab;

    private IObjectPool<Bomb> _managedPool;

    private void OnCollisionEnter(Collision other)
    {
        GameObject effectObject = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
        effectObject.transform.position = transform.position;

        ReleaseToPool();
    }

    public void SetManagedPool(IObjectPool<Bomb> pool)
    {
        _managedPool = pool;
    }

    public void ReleaseToPool()
    {
        _managedPool?.Release(this);
    }
}
