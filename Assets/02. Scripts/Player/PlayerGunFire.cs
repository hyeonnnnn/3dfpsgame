using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffect;

    [SerializeField] private float _fireCoolTime = 0.3f;
    private float _fireTimer = 0f;
    private Camera _mainCamera;

    private AmmoController _ammoController;

    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private float _shakeDuration = 0.6f;
    [SerializeField] private float _shakeMagnitude = 0.5f;

    [SerializeField] private UI_Crosshair _crosshair;

    private PlayerStats _stats;
    [SerializeField] private float _nockbackForce = 4f;

    private void Awake()
    {
        _ammoController = GetComponent<AmmoController>();
        _stats = GetComponent<PlayerStats>();
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
        if (_fireTimer < _fireCoolTime) return;
        if (_ammoController.IsReloading) return;
        if (_ammoController.RemainingAmmo <= 0 && _ammoController.CurrentMagazine <= 0) return;

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

            Monster monster = hitInfo.transform.GetComponent<Monster>();
            if (monster != null)
            {
                Vector3 direction = (hitInfo.transform.position - _fireTransform.position).normalized;
                Damage damage = new Damage(_stats.Damage.Value, direction, _nockbackForce);
                monster.TryTakeDamage(damage);
            }
        }

        _ammoController.ConsumeMagazine();
        _cameraShake.Recoil(_shakeDuration, _shakeMagnitude);
        _crosshair.Expand();
    }
}
