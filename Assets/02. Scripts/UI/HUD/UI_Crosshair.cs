using DG.Tweening;
using UnityEngine;

public class UI_Crosshair : MonoBehaviour
{
    [SerializeField] private float _expandScale = 1.5f;
    [SerializeField] private float _expandDuration = 0.1f;
    [SerializeField] private float _shrinkDuration = 0.15f;
    [SerializeField] private Ease _expandEase = Ease.OutQuad;
    [SerializeField] private Ease _shrinkEase = Ease.OutQuad;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void Expand()
    {
        transform.DOKill();

        Vector3 targetScale = _originalScale * _expandScale;

        transform.DOScale(targetScale, _expandDuration)
            .SetEase(_expandEase)
            .OnComplete(() =>
            {
                transform.DOScale(_originalScale, _shrinkDuration)
                    .SetEase(_shrinkEase);
            });
    }
}
