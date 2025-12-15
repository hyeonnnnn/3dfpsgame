using UnityEngine;

public class PlayerScan : MonoBehaviour
{
    [SerializeField] private Scan _scanEffectPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_scanEffectPrefab != null)
            {
                _scanEffectPrefab.gameObject.SetActive(true);
            }
        }
    }
}
