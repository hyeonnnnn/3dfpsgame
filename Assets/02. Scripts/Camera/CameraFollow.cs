using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _fpsViewTarget;
    [SerializeField] private Transform _tpsViewTarget;

    [SerializeField] private float _viewSwitchDuration = 0.3f;

    private bool _isTpsView = false;
    private bool _isTransitioning = false;

    private Transform _currentViewTarget;

    private void Start()
    {
        _currentViewTarget = _fpsViewTarget;
    }

    private void LateUpdate()
    {
        TrySwitchView();

        if (!_isTransitioning)
        {
            transform.position = _currentViewTarget.position;
        }
    }

    private void TrySwitchView()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isTpsView = !_isTpsView;
            _currentViewTarget = _isTpsView ? _tpsViewTarget : _fpsViewTarget;

            StartCoroutine(SwitchViewCoroutine());
        }
    }

    private IEnumerator SwitchViewCoroutine()
    {
        _isTransitioning = true;
        float progress = 0f;

        Vector3 startPosition = transform.position;

        while (progress < 1f)
        {
            progress += Time.deltaTime / _viewSwitchDuration;
            transform.position = Vector3.Lerp(startPosition, _currentViewTarget.position, progress);
            yield return null;
        }

        _isTransitioning = false;
    }
}
