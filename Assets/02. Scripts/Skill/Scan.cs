using UnityEngine;

public class Scan : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _maxScale = 100f;

    [SerializeField] private float _scanSpeed = 5f;
    [SerializeField] private float _initialScale = 1f;

    private void OnEnable()
    {
        transform.position = _player.transform.position;
    }

    private void Update()
    {
        _initialScale = Time.deltaTime;

        transform.localScale = Vector3.one * _initialScale;

        if (transform.localScale.x >= _maxScale)
        {
            gameObject.SetActive(false);
        }
    }
}
