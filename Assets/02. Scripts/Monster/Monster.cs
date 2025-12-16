using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public EMonsterState State = EMonsterState.Idle;

    private MonsterStat _stats;
    private MonsterMovement _movement;
    private MonsterCombat _combat;
    private MonsterPatrol _patrol;

    private GameObject _player;
    private PlayerController _playerController;
    private NavMeshAgent _agent;

    private Vector3 _originPosition;
    private float _attackTimer = 0f;

    private Vector3 _jumpStartPosition;
    private Vector3 _jumpEndPosition;

    private void Awake()
    {
        _stats = GetComponent<MonsterStat>();
        _movement = GetComponent<MonsterMovement>();
        _combat = GetComponent<MonsterCombat>();
        _patrol = GetComponent<MonsterPatrol>();
        _agent = GetComponent<NavMeshAgent>();
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
        if (GameManager.Instance.State == EGameState.Ready) return;
        if (GameManager.Instance.State == EGameState.GameOver) return;

        if (State == EMonsterState.Death) return;
        if (State == EMonsterState.Hit) return;

        //_movement.ApplyGravity();

        switch (State)
        {
            case EMonsterState.Idle: Idle(); break;
            case EMonsterState.Trace: Trace(); break;
            case EMonsterState.Comeback: Comeback(); break;
            case EMonsterState.Attack: Attack(); break;
            case EMonsterState.Patrol: Patrol(); break;
            case EMonsterState.Jump: Jump(); break;
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

        if (_agent.isOnOffMeshLink)
        {
            OffMeshLinkData linkData = _agent.currentOffMeshLinkData;
            _jumpStartPosition = linkData.startPos;
            _jumpEndPosition = linkData.endPos;

            if (_jumpEndPosition.y > _jumpStartPosition.y)
            {
                ChangeState(EMonsterState.Jump);
                return;
            }
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

    private void Jump()
    {
        _agent.isStopped = true;
        _agent.ResetPath();

        float jumpDistance = Vector3.Distance(_jumpStartPosition, _jumpEndPosition);
        float jumpDuration = jumpDistance / _stats.MoveSpeed.Value;

        _agent.CompleteOffMeshLink();
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
