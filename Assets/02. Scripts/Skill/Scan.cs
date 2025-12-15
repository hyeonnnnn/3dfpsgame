using UnityEngine;

public class Scan : MonoBehaviour
{
    [SerializeField] private float _maxScale = 500f;
    [SerializeField] private float _scanSpeed = 30f;
    [SerializeField] private float _initialScale = 1f;
    private float _currentScale;

    private void OnEnable()
    {
        _currentScale = _initialScale;
        gameObject.transform.localScale = Vector3.one * _initialScale;
    }

    private void Update()
    {
        _currentScale += Time.deltaTime * _scanSpeed;

        transform.localScale = Vector3.one * _currentScale;

        if (transform.localScale.x >= _maxScale)
        {
            Destroy(this.gameObject);
        }
    }
}
