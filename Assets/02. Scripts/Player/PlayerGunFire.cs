using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffect;

    [SerializeField] private List<GameObject> _muzzleEffect;

    [SerializeField] private GameObject _muzzleFlashPrefab;
    [SerializeField] private Transform _muzzlePoint;

    [SerializeField] private float _fireCoolTime = 0.3f;
    private float _fireTimer = 0f;
    private Camera _mainCamera;

    private AmmoController _ammoController;

    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private float _shakeDuration = 0.6f;
    [SerializeField] private float _shakeMagnitude = 0.5f;

    [SerializeField] private UI_Crosshair _crosshair;

    private PlayerStats _stats;
    [SerializeField] private float _knockbackForce = 4f;

    private Animator _animator;

    private void Awake()
    {
        _ammoController = GetComponent<AmmoController>();
        _stats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        _fireTimer += Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            TryFire();
        }
    }

    private void TryFire()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (GameManager.Instance.State == EGameState.Ready) return;
        if (GameManager.Instance.State == EGameState.GameOver) return;

        if (_fireTimer < _fireCoolTime) return;
        if (_ammoController.IsReloading) return;
        if (_ammoController.HasAmmunition() == false) return;

        _animator.SetTrigger("Shoot");
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
            _hitEffect.transform.position = hitInfo.point;
            _hitEffect.transform.forward = hitInfo.normal;
            _hitEffect.Play();

            Monster monster = hitInfo.transform.GetComponent<Monster>();
            if (monster != null)
            {
                Vector3 direction = (hitInfo.transform.position - _fireTransform.position).normalized;
                Damage damage = new Damage(_stats.Damage.Value, direction, _knockbackForce);
                monster.TryTakeDamage(damage);
            }

            Barrel barrel = hitInfo.transform.GetComponent<Barrel>();
            if (barrel != null)
            {
                barrel.TakeDamage(_stats.Damage.Value);
            }
        }

        _ammoController.ConsumeMagazine();
        _cameraShake.Recoil(_shakeDuration, _shakeMagnitude);
        _crosshair.Expand();
        SpawnMuzzleFlash();
    }

    private void SpawnMuzzleFlash()
    {
        if (_muzzleFlashPrefab == null) return;
        if (_muzzlePoint == null) return;

        StartCoroutine(MuzzleFlash_Coroutine());
    }

    private IEnumerator MuzzleFlash_Coroutine()
    {
        GameObject muzzleEffect = _muzzleEffect[Random.Range(0, _muzzleEffect.Count)];
        muzzleEffect.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzleEffect.SetActive(false);
    }
}
