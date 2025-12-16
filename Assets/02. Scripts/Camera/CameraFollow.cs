using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform[] _viewTarget;

    [SerializeField] private float _viewSwitchDuration = 0.3f;
    private bool _isTransitioning = false;

    private Transform _currentViewTarget;
    private int _currenttargetIndex = 0;

    private void Start()
    {
        if (_viewTarget.Length > 0)
        {
            _currentViewTarget = _viewTarget[_currenttargetIndex];
        }
    }

    private void LateUpdate()
    {
        if (_viewTarget.Length == 0) return;
        if (_currentViewTarget == null) return;

        TrySwitchView();

        if (!_isTransitioning)
        {
            transform.position = _currentViewTarget.position;
        }
    }

    private void TrySwitchView()
    {
        if (_isTransitioning) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            int direction = Input.GetKey(KeyCode.LeftShift) ? -1 : 1;
            StartCoroutine(SwitchViewCoroutine(direction));
        }
    }

    private IEnumerator SwitchViewCoroutine(int direction)
    {
        _isTransitioning = true;
        float progress = 0f;

        Vector3 startPosition = _viewTarget[_currenttargetIndex].transform.position;
        _currenttargetIndex = (_currenttargetIndex + direction + _viewTarget.Length) % _viewTarget.Length;

        Vector3 targetPosition = _viewTarget[_currenttargetIndex].transform.position;
        _currentViewTarget = _viewTarget[_currenttargetIndex];

        while (progress < 1f)
        {
            progress += Time.deltaTime / _viewSwitchDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            yield return null;
        }

        _isTransitioning = false;
    }
}
