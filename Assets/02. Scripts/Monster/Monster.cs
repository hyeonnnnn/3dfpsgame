using DG.Tweening;
using System.Collections;
using UnityEngine;

public struct Damage
{
    public float Value;
    public Vector3 Direction;
    public float KnockbackForce;

    public Damage(float value, Vector3 direction, float knockbackForce)
    {
        Value = value;
        Direction = direction;
        KnockbackForce = knockbackForce;
    }
}

public enum EMonsterMoveType
{
    Idle,
    MoveForward,
    TurnLeft,
    TurnRight,
}

[RequireComponent(typeof(Renderer))]
public class Monster : MonoBehaviour
{
    public EMonsterState State = EMonsterState.Idle;

    // 스탯
    [SerializeField] private float _health = 100f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _knockbackDuration = 0.2f;

    // 공격
    [SerializeField] private float _detectRange = 4f;
    [SerializeField] private float _traceRange = 15f;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackInterval = 2;
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _nockbackForce = 2f;

    private float _attackTimer = 0f;

    // 레퍼런스
    private GameObject _player;
    private PlayerController _playerController;
    private CharacterController _controller;
    private Renderer _renderer;

    private Vector3 _originPosition;
    private Coroutine _currentCoroutine;

    private float _patrolInterval;
    private float _patrolMaxInterval = 5f;
    private float _patrolMinInterval = 2f;
    private float _patrolTimer = 0f;
    private float _turnDuration = 0.4f;

    // 히트 이펙트
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

        if (_player != null)
        {
            _playerController = _player.GetComponent<PlayerController>();
        }

        _originPosition = transform.position;
        _originalColor = _renderer.material.color;
    }

    private void Update()
    {
        if (State == EMonsterState.Death) return;
        if (State == EMonsterState.Hit) return;

        switch (State)
        {
            case EMonsterState.Idle: Idle(); break;
            case EMonsterState.Trace: Trace(); break;
            case EMonsterState.Comeback: Comeback(); break;
            case EMonsterState.Attack: Attack(); break;
            case EMonsterState.Patrol: Patrol(); break;
        }
    }

    private void Idle()
    {
        // Todo. Idle 애니메이션 재생
        if (_player == null) return;


        if (Vector3.Distance(transform.position, _player.transform.position) <= _detectRange)
        {
            ChangeState(EMonsterState.Trace);
        }
        else
        {
            ChangeState(EMonsterState.Patrol);
        }
    }

    private void Trace()
    {
        if (_player == null)
        {
            ChangeState(EMonsterState.Comeback);
            return;
        }

        MoveTo(_player.transform.position);

        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        // Todo. Run 애니메이션 재생
        if (distanceToPlayer > _traceRange)
        {
            ChangeState(EMonsterState.Comeback);
        }

        if (distanceToPlayer <= _attackRange)
        {
            ChangeState(EMonsterState.Attack);
        }
    }

    private void Comeback()
    {
        MoveTo(_originPosition);

        float distanceToOrigin = Vector3.Distance(transform.position, _originPosition);
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (distanceToOrigin <= 1f)
        {
            ChangeState(EMonsterState.Idle);
            return;
        }
        
        if (_player == null) return;
        if (distanceToPlayer <= _detectRange)
        {
            ChangeState(EMonsterState.Attack);
        }
    }

    private void Attack()
    {
        if (_player == null)
        {
            ChangeState(EMonsterState.Idle);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        if (distanceToPlayer > _attackRange)
        {
            State = EMonsterState.Trace;
            return;
        }


        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _attackInterval)
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            PerformAttack(direction);
        }
    }

    private EMonsterMoveType _currentMoveType = EMonsterMoveType.MoveForward;

    private void Patrol()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        if (_player != null && distanceToPlayer <= _detectRange)
        {
            ChangeState(EMonsterState.Trace);
            return;
        }

        DecidePatrolAction();
        ExecutePatrolAction();
    }

    private void DecidePatrolAction()
    {
        _patrolTimer += Time.deltaTime;

        if (_patrolTimer < _patrolInterval) return;

        _patrolTimer = 0f;
        _patrolInterval = Random.Range(_patrolMinInterval, _patrolMaxInterval);

        int length = System.Enum.GetValues(typeof(EMonsterMoveType)).Length;
        _currentMoveType = (EMonsterMoveType)Random.Range(0, length);
    }

    private void ExecutePatrolAction()
    {
        switch (_currentMoveType)
        {
            case EMonsterMoveType.Idle:
                break;
            case EMonsterMoveType.MoveForward:
                _controller.Move(transform.forward * _moveSpeed * Time.deltaTime);
                break;
            case EMonsterMoveType.TurnLeft:
                transform.DORotate(new Vector3(0, transform.eulerAngles.y + 90f, 0), _turnDuration);
                _currentMoveType = EMonsterMoveType.MoveForward;
                break;
            case EMonsterMoveType.TurnRight:
                transform.DORotate(new Vector3(0, transform.eulerAngles.y - 90f, 0), _turnDuration);
                _currentMoveType = EMonsterMoveType.MoveForward;
                break;
        }
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (State == EMonsterState.Death) return false;

        _health -= damage.Value;

        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);

        if (_health > 0f)
        {
            ChangeState(EMonsterState.Hit);
            _currentCoroutine = StartCoroutine(Hit_Coroutine(damage.Direction, damage.KnockbackForce));
        }
        else
        {
            ChangeState(EMonsterState.Death);
            _currentCoroutine = StartCoroutine(Death_Coroutine());
        }
        return true;
    }

    private IEnumerator Hit_Coroutine(Vector3 direction, float knockbackForce)
    {
        float elapsed = 0f;
        Vector3 knockbackVelocity = direction * knockbackForce;

        _renderer.material.color = _hitColor;

        while (elapsed < _knockbackDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / _knockbackDuration;
            Vector3 velocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, progress);

            _controller.Move(velocity * Time.deltaTime);
            yield return null;
        }

        _renderer.material.color = _originalColor;

        yield return new WaitForSeconds(0.1f);
        ChangeState(EMonsterState.Trace);
        _currentCoroutine = null;
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private void ChangeState(EMonsterState newState)
    {
        State = newState;
    }

    private void MoveTo(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        _controller.Move(direction * _moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void PerformAttack(Vector3 direction)
    {
        _attackTimer = 0f;
        if (_playerController != null)
        {
            Damage damage = new Damage(_attackDamage, direction, _nockbackForce);
            _playerController.TakeDamage(damage);
        }
        Debug.Log("Monster Attack!");
    }
}
