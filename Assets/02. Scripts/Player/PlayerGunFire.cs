using System;
using System.Collections;
using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffect;

    [SerializeField] private float _fireCoolTime = 0.3f;
    private float _fireTimer = 0f;
    private Camera _mainCamera;

    private AmmoController _ammoController;


    private void Awake()
    {
        _ammoController = GetComponent<AmmoController>();
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

        _ammoController.ConsumeMagazine();
    }


}
