using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_HitImage : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private Image _hitImage;
    [SerializeField] private float _fadeDuration = 0.4f;

    private Coroutine _fadeCoroutine;

    private void OnEnable()
    {
        _player.OnHealthChanged += ShowHitEffect;
    }

    private void OnDisable()
    {
        _player.OnHealthChanged -= ShowHitEffect;
    }

    private void ShowHitEffect()
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(Fade_Coroutine());
    }

    private IEnumerator Fade_Coroutine()
    {
        _hitImage.color = new Color(1f, 1f, 1f, 1f);

        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / _fadeDuration);
            _hitImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        _hitImage.color = new Color(1f, 1f, 1f, 0f);
    }
}