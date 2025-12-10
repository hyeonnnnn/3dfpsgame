using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originPos;
    private Quaternion _originRot;
    private Coroutine _currentShake;

    private void Awake()
    {
        _originPos = transform.localPosition;
        _originRot = transform.localRotation;
    }

    public void Shake(float duration, float magnitude)
    {
        if (_currentShake != null)
        {
            StopCoroutine(_currentShake);
            transform.localPosition = _originPos;
        }
        _currentShake = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    public void Recoil(float duration, float magnitude)
    {
        if (_currentShake != null)
        {
            StopCoroutine(_currentShake);
            transform.localPosition = _originPos;
        }
        _currentShake = StartCoroutine(RecoilRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = _originPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = _originPos;
        _currentShake = null;
    }

    private IEnumerator RecoilRoutine(float duration, float magnitude)
    {
        Quaternion recoilRotation = _originRot * Quaternion.Euler(-magnitude, 0f, 0f);

        float elapsed = 0f;

        transform.localRotation = recoilRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - (1f - t) * (1f - t);
            transform.localRotation = Quaternion.Slerp(recoilRotation, _originRot, t);
            yield return null;
        }

        transform.localRotation = _originRot;
        _currentShake = null;
    }
}