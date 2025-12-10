using System;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerBombFire : MonoBehaviour
{
    [SerializeField] private Transform _fireTransform;

    [Header("폭탄")]
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private int _bombMaxcount = 5;
    [SerializeField] private int _bombCount;
    [SerializeField] private Transform _bombParent;

    [SerializeField] private float _throwPower = 15f;
    private Transform _cameraTransform;

    public event Action<int, int> OnBombCountChanged;

    private IObjectPool<Bomb> _bombPool;
    private int _maxCount = 15;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _bombPool = new ObjectPool<Bomb>(CreateBomb, OnGetBomb, OnReleaseBomb, OnDestroyBomb, maxSize: _maxCount);
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
            OnBombCountChanged?.Invoke(_bombCount, _bombMaxcount);
        }
    }

    private void Fire()
    {
        var bomb = _bombPool.Get();
        bomb.transform.position = _fireTransform.position;

        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.AddForce(_cameraTransform.forward * _throwPower, ForceMode.VelocityChange);
    }

    private Bomb CreateBomb()
    {
        Bomb bomb = Instantiate(_bombPrefab, _bombParent);
        bomb.SetManagedPool(_bombPool);
        return bomb;
    }

    private void OnGetBomb(Bomb bomb)
    {
        bomb.gameObject.SetActive(true);
    }

    private void OnReleaseBomb(Bomb bomb)
    {
        bomb.gameObject.SetActive(false);
    }

    private void OnDestroyBomb(Bomb bomb)
    {
        Destroy(bomb.gameObject);
    }
}
