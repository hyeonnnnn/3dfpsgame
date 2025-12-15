using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MonsterStat))]
public class MonsterHeathBar : MonoBehaviour
{
    private MonsterStat _stat;

    [Header("체력 게이지")]
    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _guageImage;
    private Color _originalColor;

    [Header("딜레이 게이지")]
    [SerializeField] private Image _delayGuageImage;
    [SerializeField] private float _guageSpeed = 3f;

    [Header("피격 효과")]
    [SerializeField] private Color _hitColor = new Color(1f, 1f, 1f, 0.5f);

    [Header("떨림 효과")]
    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeStrength = 0.15f;

    private float _lastHealth = -1;
    private Coroutine _delayCoroutine;
    private Coroutine _hitEffectCoroutine;

    private Camera _mainCamera;

    private void Awake()
    {
        _stat = GetComponent<MonsterStat>();
        _originalColor = _guageImage.color;
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_lastHealth != _stat.Health.Value)
        {
            _lastHealth = _stat.Health.Value;
            _guageImage.fillAmount = _stat.Health.Value / _stat.Health.MaxValue;

            if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
            if (_hitEffectCoroutine != null) StopCoroutine(_hitEffectCoroutine);

            _delayCoroutine = StartCoroutine(DelayGauge_Coroutine());
            _hitEffectCoroutine = StartCoroutine(HitEffect_Coroutine());
        }

        _healthBarTransform.forward = _mainCamera.transform.forward;
    }

    private IEnumerator DelayGauge_Coroutine()
    {
        yield return new WaitForSeconds(0.2f);

        while (Mathf.Approximately(_delayGuageImage.fillAmount, _guageImage.fillAmount) == false)
        {
            _delayGuageImage.fillAmount = Mathf.MoveTowards(
                _delayGuageImage.fillAmount,
                _guageImage.fillAmount,
                Time.deltaTime * _guageSpeed
            );
            yield return null;
        }
    }

    private IEnumerator HitEffect_Coroutine()
    {
        _guageImage.color = _hitColor;

        _healthBarTransform.DOShakePosition(_shakeDuration, _shakeStrength);

        yield return new WaitForSeconds(0.1f);
        _guageImage.color = _originalColor;
    }
}
