using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MonsterStat))]
public class MonsterHealthBar : MonoBehaviour
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
    private const float HitEffectDuration = 0.1f;
    private const float DelayGuageDelay = 0.2f;

    [Header("떨림 효과")]
    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeStrength = 0.15f;

    private Vector3 _originalLocalPosition;
    private Tweener _shakeTweener;
    private Coroutine _delayCoroutine;
    private Coroutine _hitEffectCoroutine;

    private Camera _mainCamera;

    private void Awake()
    {
        _stat = GetComponent<MonsterStat>();
        _originalColor = _guageImage.color;
        _originalLocalPosition = _healthBarTransform.localPosition;
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        _stat.OnHealthChanged += Refresh;
    }

    private void OnDisable()
    {
        _stat.OnHealthChanged -= Refresh;
    }

    private void LateUpdate()
    {
        _healthBarTransform.forward = _mainCamera.transform.forward;
    }

    private void Refresh()
    {
        _guageImage.fillAmount = _stat.Health.Value / _stat.Health.MaxValue;

        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        if (_hitEffectCoroutine != null) StopCoroutine(_hitEffectCoroutine);

        _delayCoroutine = StartCoroutine(DelayGauge_Coroutine());
        _hitEffectCoroutine = StartCoroutine(HitEffect_Coroutine());
    }

    private IEnumerator DelayGauge_Coroutine()
    {
        yield return new WaitForSeconds(DelayGuageDelay);

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

        _shakeTweener?.Kill();
        _healthBarTransform.localPosition = _originalLocalPosition;
        _shakeTweener = _healthBarTransform.DOShakePosition(_shakeDuration, _shakeStrength);

        yield return new WaitForSeconds(HitEffectDuration);
        _guageImage.color = _originalColor;
    }
}
