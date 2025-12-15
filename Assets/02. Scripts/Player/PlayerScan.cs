using UnityEngine;

public class PlayerScan : MonoBehaviour
{
    [SerializeField] private GameObject _scanEffectPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_scanEffectPrefab != null)
            {
                Instantiate(_scanEffectPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
