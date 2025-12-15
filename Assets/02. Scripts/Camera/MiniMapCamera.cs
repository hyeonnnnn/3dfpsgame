using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _offsetY = 8f;

    private Camera _camera;

    public float OffsetY => _offsetY;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        SetOffsetY(_offsetY);
    }

    private void LateUpdate()
    {
        if (_targetTransform == null) return;

        Vector3 targetPosition = _targetTransform.position;
        Vector3 finalPosition = targetPosition + new Vector3(0f, _offsetY, 0f);

        transform.position = finalPosition;
        
        Vector3 targetAngle = _targetTransform.eulerAngles;
        targetAngle.x = 90f;

        transform.eulerAngles = targetAngle;
    }

    public void SetOffsetY(float offsetY)
    {
        _offsetY = offsetY;
        _camera.orthographicSize = offsetY;
    }
}
