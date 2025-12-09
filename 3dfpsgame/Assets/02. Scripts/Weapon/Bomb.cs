using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject _explosionEffectPrefab;

    private void OnCollisionEnter(Collision other)
    {
        GameObject effectObject = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
        effectObject.transform.position = transform.position;

        Destroy(gameObject);
    }
}
