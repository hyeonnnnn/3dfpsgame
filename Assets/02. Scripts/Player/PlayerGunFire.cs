using System;
using System.Collections;
using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffect;
    
    [SerializeField] private int _currentMagazine;
    [SerializeField] private int _maxMagazine = 30;
    [SerializeField] private int _remainingAmmo = 120;
    [SerializeField] private float _reloadCoolTime = 1.6f;

    [SerializeField] private float _fireCoolTime = 0.3f;
    private float _fireTimer = 0f;
    private Camera _mainCamera;

    public event Action<int, int> OnBulletCountChanged;
    public event Action<float> OnReloaded;

    private void Start()
    {
        _mainCamera = Camera.main;
        _currentMagazine = _maxMagazine;
    }

    private void Update()
    {
        _fireTimer += Time.deltaTime;

        // 발사
        if (Input.GetMouseButton(0))
        {
            TryFire();
        }

        // 재장전
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReloadBullet();
        }
    }

    private void TryFire()
    {
        if (_fireTimer < _fireCoolTime) return;

        if (_currentMagazine <= 0)
        {
            TryReloadBullet();
            return;
        }

        Ray ray = new Ray(_fireTransform.position, _mainCamera.transform.forward);
        RaycastHit hitInfo = new RaycastHit();
        Fire(ray, hitInfo);

        _fireTimer = 0f;
    }

    private void Fire(Ray ray, RaycastHit hitInfo)
    {
        bool isHit = Physics.Raycast(ray, out hitInfo);

        if (isHit)
        {
            Debug.Log($"Hit : {hitInfo.transform.name}");
            _hitEffect.transform.position = hitInfo.point;
            _hitEffect.transform.forward = hitInfo.normal;
            _hitEffect.Play();
        }

        _currentMagazine--;
        OnBulletCountChanged?.Invoke(_currentMagazine, _remainingAmmo);
    }

    private void TryReloadBullet()
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
        ReloadBullet(neededBullets);
    }

    private void ReloadBullet(int neededBullets)
    {
        _remainingAmmo -= neededBullets;
        _currentMagazine += neededBullets;

        OnBulletCountChanged?.Invoke(_currentMagazine, _remainingAmmo);
    }
}
