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

    [Header("라인 렌더러")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _lineDisplayDuration = 0.05f;
    [SerializeField] private float _maxRayDistance = 100f;

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

    private EZoomMode _currentZoomMode = EZoomMode.Normal;
    [SerializeField] private GameObject _normalCrosshair;
    [SerializeField] private GameObject _zoomInCrosshair;

    private void Awake()
    {
        _ammoController = GetComponent<AmmoController>();
        _stats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _mainCamera = Camera.main;

        if (_lineRenderer != null)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        _fireTimer += Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            TryFire();
        }
        CheckZoomMode();
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

        Vector3 endPoint;
        if (isHit)
        {
            endPoint = hitInfo.point;

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
        else
        {
            endPoint = ray.origin + ray.direction * _maxRayDistance;
        }

        DrawBulletTrail(_muzzlePoint.position, endPoint);

        _ammoController.ConsumeMagazine();
        // _cameraShake.Recoil(_shakeDuration, _shakeMagnitude);
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

    private void DrawBulletTrail(Vector3 startPoint, Vector3 endPoint)
    {
        if (_lineRenderer == null) return;

        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, endPoint);
        _lineRenderer.enabled = true;

        StartCoroutine(HideBulletTrail_Coroutine());
    }

    private IEnumerator HideBulletTrail_Coroutine()
    {
        yield return new WaitForSeconds(_lineDisplayDuration);
        _lineRenderer.enabled = false;
    }

    private void CheckZoomMode()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _currentZoomMode = EZoomMode.ZoomIn;
            _normalCrosshair.SetActive(false);
            _zoomInCrosshair.SetActive(true);
            Camera.main.fieldOfView = 20f;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            _currentZoomMode = EZoomMode.Normal;
            _normalCrosshair.SetActive(true);
            _zoomInCrosshair.SetActive(false);
            Camera.main.fieldOfView = 60f;
        }
    }
}
