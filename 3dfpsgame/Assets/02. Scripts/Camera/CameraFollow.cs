using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _fpsViewTarget;
    [SerializeField] private Transform _tpsViewTarget;
    [SerializeField] private float _smoothTime = 0.3f;

    private bool _isTpsView = false;
    private Vector3 _velocity = Vector3.zero;

    private void LateUpdate()
    {
        TrySwitchView();
    }

    private void TrySwitchView()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isTpsView = !_isTpsView;
        }

        Transform target = _isTpsView ? _tpsViewTarget : _fpsViewTarget;
        if (target == null) return;

        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref _velocity, _smoothTime);
    }
}
