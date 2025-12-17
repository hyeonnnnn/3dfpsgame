using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Monster : MonoBehaviour
{
    private const float ORIGIN_ARRIVAL_THRESHOLD = 1f;
    private const float JUMP_HEIGHT_OFFSET = 0.5f;
    private const float PARABOLA_MULTIPLIER = 4f;

    public EMonsterState State = EMonsterState.Idle;

    private MonsterStat _monsterStat;
    private MonsterMove _monsterMovement;
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
    private float _jumpDuration = 0.5f;


    private void Awake()
    {
        _monsterStat = GetComponent<MonsterStat>();
        _monsterMovement = GetComponent<MonsterMove>();
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

        _monsterMovement.ApplyGravity();

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
            ChangeState(EMonsterState.Jump);
            return;
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

        _isJumping = true;
        _navMeshAgent.isStopped = true;

        OffMeshLinkData linkData = _navMeshAgent.currentOffMeshLinkData;
        _jumpStartPosition = linkData.startPos;
        _jumpEndPosition = linkData.endPos;

        StartCoroutine(Jump_Coroutine());
    }

    private IEnumerator Jump_Coroutine()
    {
        _navMeshAgent.CompleteOffMeshLink();

        float elapsed = 0f;
        Vector3 startPos = _jumpStartPosition;
        Vector3 endPos = _jumpEndPosition;

        float heightDifference = endPos.y - startPos.y;
        float actualJumpHeight = _monsterStat.JumpHeight.Value + Mathf.Max(0, heightDifference * JUMP_HEIGHT_OFFSET);

        while (elapsed < _jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _jumpDuration;
            Vector3 horizontalPos = Vector3.Lerp(startPos, endPos, t);

            float parabola = PARABOLA_MULTIPLIER * t * (1f - t);
            float verticalOffset = parabola * actualJumpHeight;

            transform.position = horizontalPos + Vector3.up * verticalOffset;

            yield return null;
        }

        transform.position = endPos;

        _isJumping = false;
        _navMeshAgent.isStopped = false;

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
