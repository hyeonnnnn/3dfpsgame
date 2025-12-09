using System;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [SerializeField] private Transform _fireTransform;

    [Header("폭탄")]
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private int _bombMaxcount = 5;
    [SerializeField] private int _bombCount;

    [SerializeField] private float _throwPower = 15f;
    private Transform _cameraTransform;

    public event Action<int> OnBombCountChanged;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        _bombCount = _bombMaxcount;
    }

    private void Update()
    {
        TryFire();
    }

    private void TryFire()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (_bombCount <= 0) return;
            Fire();
            _bombCount--;
            OnBombCountChanged?.Invoke(_bombCount);
        }
    }

    private void Fire()
    {
        Bomb bomb = Instantiate(_bombPrefab, _fireTransform.position, Quaternion.identity);
        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();

        rigidbody.AddForce(_cameraTransform.forward * _throwPower, ForceMode.VelocityChange);
    }
}
