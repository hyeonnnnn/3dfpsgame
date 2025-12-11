using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{
    public EMonsterState State = EMonsterState.Idle;

    [SerializeField] private float _health = 100f;
    [SerializeField] private float _detectRange = 4f;
    [SerializeField] private float _attackRange = 1.2f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _attackInterval = 2;
    [SerializeField] private float _attackDamage = 10f;
    private float _attackTimer = 0f;

    private GameObject _player;
    private PlayerController _playerController;
    private CharacterController _controller;

    private Vector3 _originPosition;

    private Renderer _renderer;
    private Color _hitColor = Color.red;
    private Color _originalColor;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _originPosition = transform.position;
        _originalColor = _renderer.material.color;
    }

    private void Update()
    {
        if (_player == null) return;

        switch (State)
        {
            case EMonsterState.Idle: Idle(); break;
            case EMonsterState.Trace: Trace(); break;
            case EMonsterState.Comeback: Comeback(); break;
            case EMonsterState.Attack: Attack(); break;
        }
    }

    private void Idle()
    {
        // Todo. Idle 애니메이션 재생
        if (Vector3.Distance(transform.position, _player.transform.position) <= _detectRange)
        {
            State = EMonsterState.Trace;
        }
    }

    private void Trace()
    {
        Vector2 direction = (_player.transform.position - transform.position).normalized;
        _controller.Move(new Vector3(direction.x, 0, direction.y) * _moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        // Todo. Run 애니메이션 재생
        if (distance > _detectRange)
        {
            State = EMonsterState.Idle;
        }

        if (distance <= _attackRange)
        {
            State = EMonsterState.Attack;
        }
    }

    private void Comeback()
    {
        // 과제. Comeback 상태 구현
    }

    private void Attack()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > _attackRange)
        {
            State = EMonsterState.Trace;
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _attackInterval)
        {
            _attackTimer = 0f;
            _playerController.TakeDamage(_attackDamage);
            Debug.Log("Monster Attack!");
        }
    }

    public bool TryTakeDamage(float damage, Vector3 direction, float nockbackForce)
    {
        if (State == EMonsterState.Death) return false;

        _health -= damage;

        if (_health > 0f)
        {
            State = EMonsterState.Hit;
            StartCoroutine(Hit_Coroutine(direction, nockbackForce));
        }
        else
        {
            State = EMonsterState.Death;
            StartCoroutine(Death_Coroutine());
        }
        return true;
    }

    private IEnumerator Hit_Coroutine(Vector3 direction, float knockbackForce)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        Vector3 knockbackVelocity = direction * knockbackForce;

        _renderer.material.color = _hitColor;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            Vector3 velocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, progress);
            _controller.Move(velocity * Time.deltaTime);
            yield return null;
        }
        _renderer.material.color = _originalColor;

        yield return new WaitForSeconds(0.3f);
        State = EMonsterState.Idle;
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
