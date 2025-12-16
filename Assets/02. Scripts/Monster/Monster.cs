using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Monster : MonoBehaviour
{
    private const float ORIGIN_ARRIVAL_THRESHOLD = 1f;
    private const float MIN_JUMP_HEIGHT = 1.5f;
    private const float JUMP_HEIGHT_OFFSET = 0.5f;
    private const float PARABOLA_MULTIPLIER = 4f;

    public EMonsterState State = EMonsterState.Idle;

    private MonsterStat _monsterStat;
    private MonsterMovement _monsterMovement;
    private MonsterCombat _monsterCombat;
    private MonsterPatrol _monsterPatrol;

    private GameObject _player;
    private PlayerController _playerController;
    private NavMeshAgent _navMeshAgent;

    private Vector3 _originPosition;
    private float _attackTimer = 0f;

    private Vector3 _jumpStartPosition;
    private Vector3 _jumpEndPosition;
    private bool _isJumping = false;

    private void Awake()
    {
        _monsterStat = GetComponent<MonsterStat>();
        _monsterMovement = GetComponent<MonsterMovement>();
        _monsterCombat = GetComponent<MonsterCombat>();
        _monsterPatrol = GetComponent<MonsterPatrol>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        _monsterCombat.OnHitComplete += HandleHitComplete;
        _monsterCombat.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        _monsterCombat.OnHitComplete -= HandleHitComplete;
        _monsterCombat.OnDeath -= HandleDeath;
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

        ChangeState(_monsterStat.Health.Value - damage.Value > 0f ? EMonsterState.Hit : EMonsterState.Death);
        return _monsterCombat.TryTakeDamage(damage);
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

        if (GetDistanceToPlayer() <= _monsterStat.DetectRange.Value)
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

        _monsterMovement.MoveTo(_player.transform.position);

        float distanceToPlayer = GetDistanceToPlayer();

        if (distanceToPlayer > _monsterStat.TraceRange.Value)
        {
            ChangeState(EMonsterState.Comeback);
        }

        if (distanceToPlayer <= _monsterStat.AttackRange.Value)
        {
            ChangeState(EMonsterState.Attack);
        }

        if (_navMeshAgent.isOnOffMeshLink)
        {
            OffMeshLinkData linkData = _navMeshAgent.currentOffMeshLinkData;
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
        _monsterMovement.MoveTo(_originPosition);

        float distanceToOrigin = Vector3.Distance(transform.position, _originPosition);

        if (distanceToOrigin <= ORIGIN_ARRIVAL_THRESHOLD)
        {
            ChangeState(EMonsterState.Idle);
            return;
        }

        if (_player == null) return;
        if (GetDistanceToPlayer() <= _monsterStat.DetectRange.Value)
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

        if (GetDistanceToPlayer() > _monsterStat.AttackRange.Value)
        {
            ChangeState(EMonsterState.Trace);
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _monsterStat.AttackInterval.Value)
        {
            _attackTimer = 0f;
            Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
            _monsterCombat.PerformAttack(_playerController, directionToPlayer);
        }
    }

    private void Patrol()
    {
        if (_player != null && GetDistanceToPlayer() <= _monsterStat.DetectRange.Value)
        {
            ChangeState(EMonsterState.Trace);
            return;
        }

        _monsterPatrol.UpdatePatrol();
    }

    private void Jump()
    {
        if (_isJumping) return;

        _navMeshAgent.isStopped = true;
        StartCoroutine(JumpCoroutine());
    }

    private IEnumerator JumpCoroutine()
    {
        _isJumping = true;

        float jumpDistance = Vector3.Distance(_jumpStartPosition, _jumpEndPosition);
        float jumpDuration = jumpDistance / _monsterStat.MoveSpeed.Value;
        float heightDifference = _jumpEndPosition.y - _jumpStartPosition.y;
        float jumpHeight = Mathf.Max(MIN_JUMP_HEIGHT, heightDifference + JUMP_HEIGHT_OFFSET);

        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / jumpDuration;

            Vector3 horizontalPosition = Vector3.Lerp(_jumpStartPosition, _jumpEndPosition, normalizedTime);
            float baseY = Mathf.Lerp(_jumpStartPosition.y, _jumpEndPosition.y, normalizedTime);
            float yOffset = jumpHeight * PARABOLA_MULTIPLIER * normalizedTime * (1f - normalizedTime);
            transform.position = new Vector3(horizontalPosition.x, baseY + yOffset, horizontalPosition.z);

            yield return null;
        }

        transform.position = _jumpEndPosition;
        _navMeshAgent.CompleteOffMeshLink();
        _navMeshAgent.isStopped = false;
        _isJumping = false;

        ChangeState(EMonsterState.Trace);
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
