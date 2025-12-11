using System.Collections;
using UnityEngine;

public class UI_Crosshair : MonoBehaviour
{
    [SerializeField] private float _expandScale = 1.5f;
    [SerializeField] private float _expandDuration = 0.1f;
    [SerializeField] private float _shrinkDuration = 0.15f;

    private Vector3 _originalScale;
    private Coroutine _currentCoroutine;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void Expand()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        _currentCoroutine = StartCoroutine(ExpandCoroutine());
    }

    private IEnumerator ExpandCoroutine()
    {
        Vector3 targetScale = _originalScale * _expandScale;

        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < _expandDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / _expandDuration);
            yield return null;
        }
        transform.localScale = targetScale;

        elapsed = 0f;
        while (elapsed < _shrinkDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, _originalScale, elapsed / _shrinkDuration);
            yield return null;
        }
        transform.localScale = _originalScale;

        _currentCoroutine = null;
    }
}
