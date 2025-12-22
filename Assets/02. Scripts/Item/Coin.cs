using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("회전")]
    [SerializeField] private float _rotateSpeed = 180f;

    [Header("획득")]
    [SerializeField] private int _coinValue = 1;

    [Header("플레이어 감지")]
    [SerializeField] private float _detectRadius = 3f;
    [SerializeField] private float _coolTime = 0.5f;

    [Header("아이템 이동")]
    [SerializeField] private float _flySpeed = 5f;

    [Header("베지어곡선 제어")]
    [SerializeField] private float _controlPointHeightMin = 1f;
    [SerializeField] private float _controlPointHeightMax = 3f;
    [SerializeField] private float _controlPointOffsetMin = -1f;
    [SerializeField] private float _controlPointOffsetMax = 1f;

    private Transform _playerTransform;
    private Vector3 _startPoint;
    private Vector3 _controlPoint;

    private float _curveProgression = 0f;
    private float _timer = 0f;
    private bool _isFlying = false;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        _timer += Time.deltaTime;

        if (!_isFlying)
        {
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            if (_timer >= _coolTime && distance <= _detectRadius)
            {
                StartBezierFly();
            }
        }
        else
        {
            MoveAlongBezier();
        }
    }

    private void StartBezierFly()
    {
        _isFlying = true;
        _startPoint = transform.position;

        Vector3 midPoint = (_startPoint + _playerTransform.position) / 2f;

        float heightOffset = Random.Range(_controlPointHeightMin, _controlPointHeightMax);
        float xOffset = Random.Range(_controlPointOffsetMin, _controlPointOffsetMax);
        float zOffset = Random.Range(_controlPointOffsetMin, _controlPointOffsetMax);

        _controlPoint = new Vector3(midPoint.x + xOffset, midPoint.y + heightOffset, midPoint.z + zOffset);
        _curveProgression = 0f;
    }

    private void MoveAlongBezier()
    {
        _curveProgression += Time.deltaTime * _flySpeed;
        _curveProgression = Mathf.Clamp01(_curveProgression);

        Vector3 endPoint = _playerTransform.position;

        float t = _curveProgression;
        float oneMinusT = 1f - t;

        Vector3 bezierPosition = oneMinusT * oneMinusT * _startPoint +
                                  2f * oneMinusT * t * _controlPoint +
                                  t * t * endPoint;

        transform.position = bezierPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        Destroy(gameObject);
    }
}
