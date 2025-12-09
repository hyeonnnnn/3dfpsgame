using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _fpsViewTarget;
    [SerializeField] private Transform _tpsViewTarget;
    [SerializeField] private float _smoothTime = 0.3f;

    private bool _canSwitch = false;

    private void LateUpdate()
    {
        TrySwitchView();
    }

    private void TrySwitchView()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _canSwitch = !_canSwitch;
        }

        if (_canSwitch == false)
        {
            SwitchView(_fpsViewTarget);
        }
        else
        {
            SwitchView(_tpsViewTarget);
        }
    }

    private void SwitchView(Transform target)
    {
        transform.DOMove(target.position, _smoothTime);
    }
}
