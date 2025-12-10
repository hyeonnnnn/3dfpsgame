using System;
using UnityEngine;
using System.Collections;

public class AmmoController : MonoBehaviour
{
    [SerializeField] private int _currentMagazine;
    [SerializeField] private int _maxMagazine = 30;
    [SerializeField] private int _remainingAmmo = 120;
    [SerializeField] private float _reloadCoolTime = 1.6f;

    public int CurrentMagazine => _currentMagazine;

    public event Action<float> OnReloaded;
    public event Action<int, int> OnAmmoCountChanged;

    private void Start()
    {
        _currentMagazine = _maxMagazine;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReload();
        }
    }

    private void TryReload()
    {
        int neededBullets = _maxMagazine - _currentMagazine;
        if (neededBullets <= 0) return;
        if (_remainingAmmo <= 0) return;

        if (neededBullets > _remainingAmmo)
        {
            neededBullets = _remainingAmmo;
        }

        StartCoroutine(ReloadCoroutine(neededBullets));
    }

    private IEnumerator ReloadCoroutine(int neededBullets)
    {
        float timer = 0f;
        OnReloaded?.Invoke(_reloadCoolTime);
        while (timer < _reloadCoolTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Reload(neededBullets);
    }

    private void Reload(int neededBullets)
    {
        _remainingAmmo -= neededBullets;
        _currentMagazine += neededBullets;

        OnAmmoCountChanged?.Invoke(_currentMagazine, _remainingAmmo);
    }

    public void ConsumeMagazine()
    {
        if (_currentMagazine <= 0)
        {
            TryReload();
            return;
        }
        _currentMagazine--;
        OnAmmoCountChanged?.Invoke(_currentMagazine, _remainingAmmo);
    }
}
