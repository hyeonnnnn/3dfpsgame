using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffectPrefab;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        TryFire();
    }

    private void TryFire()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(_fireTransform.position, _mainCamera.transform.forward);
            RaycastHit hitInfo = new RaycastHit(); // 충돌한 대상의 정보를 저장
            Fire(ray, hitInfo);
        }
    }

    private void Fire(Ray ray, RaycastHit hitInfo)
    {
        bool isHit = Physics.Raycast(ray, out hitInfo);

        if (isHit)
        {
            Debug.Log($"Hit : {hitInfo.transform.name}");
            ParticleSystem hitEffect = Instantiate(_hitEffectPrefab, hitInfo.point, Quaternion.identity);
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Play();
        }
    }

    // Ray: 레이저 (시작 위치, 방향, 거리)
    // hitInfo: 충돌한 대상의 정보 저장
    // RaycastHit: 충돌한 대상의 정보 저장
}
