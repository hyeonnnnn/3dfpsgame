using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Quaternion _originRotation;
    private Coroutine _currentShake;

    private void Awake()
    {
        _originRotation = transform.localRotation;
    }

    public void Recoil(float duration, float magnitude)
    {
        if (_currentShake != null)
        {
            StopCoroutine(_currentShake);
            transform.localRotation = _originRotation;
        }
        _currentShake = StartCoroutine(Recoil_Coroutine(duration, magnitude));
    }

    private IEnumerator Recoil_Coroutine(float duration, float magnitude)
    {
        Quaternion recoilRotation = _originRotation * Quaternion.Euler(-magnitude, 0f, 0f);

        float elapsed = 0f;

        transform.localRotation = recoilRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - (1f - t) * (1f - t);
            transform.localRotation = Quaternion.Slerp(recoilRotation, _originRotation, t);
            yield return null;
        }

        transform.localRotation = _originRotation;
        _currentShake = null;
    }
}