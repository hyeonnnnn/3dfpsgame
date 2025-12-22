using UnityEngine;

public class MonsterItemDrop : MonoBehaviour
{
    [SerializeField] private GameObject _coinPrefab;

    [Header("드랍 개수")]
    [SerializeField] private int _coinMinCount = 1;
    [SerializeField] private int _coinMaxCount = 4;

    [Header("드랍 범위")]
    [SerializeField] private float _dropRadius = 1f;

    private bool _hasDropped = false;

    public void DropItem()
    {
        if (_hasDropped) return;
        _hasDropped = true;

        int coinCount = Random.Range(_coinMinCount, _coinMaxCount + 1);

        for (int i = 0; i < coinCount; i++)
        {
            Vector3 randomOffset = new Vector3 (Random.Range(-_dropRadius, _dropRadius), 0f, Random.Range(-_dropRadius, _dropRadius));

            Instantiate(_coinPrefab, transform.position + randomOffset, Quaternion.identity);
        }
    }
}
