using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _fpsViewTarget;
    [SerializeField] private Transform _tpsViewTarget;

    [SerializeField] private float _viewSwitchDuration = 0.3f;

    private bool _isTpsView = false;
    private bool _isTransitioning = false;
    private float _transitionProgress = 0f;

    private Transform _currentViewTarget;
    private Tween _switchTween;

    private void Start()
    {
        _currentViewTarget = _fpsViewTarget;
    }

    private void LateUpdate()
    {
        TrySwitchView();
        FollowTarget();
    }

    private void TrySwitchView()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isTpsView = !_isTpsView;
            _currentViewTarget = _isTpsView ? _tpsViewTarget : _fpsViewTarget;

            if (_switchTween != null && _switchTween.IsActive()) _switchTween.Kill();

            _isTransitioning = true;
            _transitionProgress = 0f;

            // 진행 관리용으로만 사용
            _switchTween = DOTween.To(
                () => _transitionProgress,
                x => _transitionProgress = x,
                1f,
                _viewSwitchDuration
            ).SetEase(Ease.OutSine)
            .OnComplete(() => _isTransitioning = false);
        }
    }

    private void FollowTarget()
    {
        if (_currentViewTarget == null) return;

        if (_isTransitioning)
        {
            // 실제 위치 보간
            transform.position = Vector3.Lerp(transform.position, _currentViewTarget.position, _transitionProgress);
        }
        else
        {
            transform.position = _currentViewTarget.position;
        }
    }
}
