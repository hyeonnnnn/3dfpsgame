using UnityEngine;

public class Monster : MonoBehaviour
{
    public EMonsterState State = EMonsterState.Idle;

    private MonsterStats _stats;
    private MonsterMovement _movement;
    private MonsterCombat _combat;
    private MonsterPatrol _patrol;

    private GameObject _player;
    private PlayerController _playerController;

    private Vector3 _originPosition;
    private float _attackTimer = 0f;

    private void Awake()
    {
        _stats = GetComponent<MonsterStats>();
        _movement = GetComponent<MonsterMovement>();
        _combat = GetComponent<MonsterCombat>();
        _patrol = GetComponent<MonsterPatrol>();
    }

    private void OnEnable()
    {
        _combat.OnHitComplete += HandleHitComplete;
        _combat.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        _combat.OnHitComplete -= HandleHitComplete;
        _combat.OnDeath -= HandleDeath;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        if (_player != null)
        {
            _playerController = _player.GetComponent<PlayerController>();
        }

        _originPosition = transform.position;
    }

    private void Update()
    {
        if (State == EMonsterState.Death) return;
        if (State == EMonsterState.Hit) return;

        _movement.ApplyGravity();

        switch (State)
        {
            case EMonsterState.Idle: Idle(); break;
            case EMonsterState.Trace: Trace(); break;
            case EMonsterState.Comeback: Comeback(); break;
            case EMonsterState.Attack: Attack(); break;
            case EMonsterState.Patrol: Patrol(); break;
        }
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (State == EMonsterState.Death) return false;

        ChangeState(_stats.Health.Value - damage.Value > 0f ? EMonsterState.Hit : EMonsterState.Death);
        return _combat.TryTakeDamage(damage);
    }

    private void HandleHitComplete()
    {
        ChangeState(EMonsterState.Trace);
    }

    private void HandleDeath()
    {
        ChangeState(EMonsterState.Death);
    }

    private void Idle()
    {
        if (_player == null) return;

        if (GetDistanceToPlayer() <= _stats.DetectRange.Value)
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

        _movement.MoveTo(_player.transform.position);

        float distanceToPlayer = GetDistanceToPlayer();

        if (distanceToPlayer > _stats.TraceRange.Value)
        {
            ChangeState(EMonsterState.Comeback);
        }

        if (distanceToPlayer <= _stats.AttackRange.Value)
        {
            ChangeState(EMonsterState.Attack);
        }
    }

    private void Comeback()
    {
        _movement.MoveTo(_originPosition);

        float distanceToOrigin = Vector3.Distance(transform.position, _originPosition);

        if (distanceToOrigin <= 1f)
        {
            ChangeState(EMonsterState.Idle);
            return;
        }

        if (_player == null) return;
        if (GetDistanceToPlayer() <= _stats.DetectRange.Value)
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

        if (GetDistanceToPlayer() > _stats.AttackRange.Value)
        {
            ChangeState(EMonsterState.Trace);
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _stats.AttackInterval.Value)
        {
            _attackTimer = 0f;
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            _combat.PerformAttack(_playerController, direction);
        }
    }

    private void Patrol()
    {
        if (_player != null && GetDistanceToPlayer() <= _stats.DetectRange.Value)
        {
            ChangeState(EMonsterState.Trace);
            return;
        }

        _patrol.UpdatePatrol();
    }

    private void ChangeState(EMonsterState newState)
    {
        State = newState;
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _player.transform.position);
    }
}
