using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MiniMap : MonoBehaviour
{
    [SerializeField] private MiniMapCamera _miniMapCamera;

    [SerializeField] private float _zoomStep = 1f;
    [SerializeField] private float _minZoom = 4f;
    [SerializeField] private float _maxZoom = 16f;

    [SerializeField] private Button _zoomInButton;
    [SerializeField] private Button _zoomOutButton;

    private float _currentZoom;

    private void Awake()
    {
        _currentZoom = _miniMapCamera.OffsetY;

        _zoomInButton.onClick.AddListener(HandleZoomInButtonClicked);
        _zoomOutButton.onClick.AddListener(HandleZoomOutButtonClicked);
    }

    public void HandleZoomInButtonClicked()
    {
        _currentZoom = Mathf.Clamp(_currentZoom - _zoomStep, _minZoom, _maxZoom);
        _miniMapCamera.SetOffsetY(_currentZoom);
    }

    public void HandleZoomOutButtonClicked()
    {
        _currentZoom = Mathf.Clamp(_currentZoom + _zoomStep, _minZoom, _maxZoom);
        _miniMapCamera.SetOffsetY(_currentZoom);
    }
}
