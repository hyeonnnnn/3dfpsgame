using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _fpsViewTarget;
    [SerializeField] private Transform _tpsViewTarget;
    [SerializeField] private float _smoothTime = 0.1f;

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

        transform.DOMove(target.position, _smoothTime);
    }
}
