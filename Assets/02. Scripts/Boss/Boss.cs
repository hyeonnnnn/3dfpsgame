using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    private const float JUMP_DURATION = 1.0f;
    private const float JUMP_HEIGHT = 5f;
    private const float DEATH_DISAPPEAR_DELAY = 2f;

    public EBossState State = EBossState.Idle;

    private BossStat _bossStat;
    private BossMove _bossMove;
    private BossCombat _bossCombat;
    private Animator _animator;

    private GameObject _player;
    private PlayerController _playerController;

    private float _attackTimer = 0f;
    private float _jumpCooldownTimer = 0f;
    private bool _isJumping = false;
    private Vector3 _jumpTargetPosition;

    private void Awake()
    {
        _bossStat = GetComponent<BossStat>();
        _bossMove = GetComponent<BossMove>();
        _bossCombat = GetComponent<BossCombat>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        _bossCombat.OnHitComplete += HandleHitComplete;
        _bossCombat.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        _bossCombat.OnHitComplete -= HandleHitComplete;
        _bossCombat.OnDeath -= HandleDeath;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        if (_player != null)
        {
            _playerController = _player.GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State == EGameState.Ready) return;
        if (GameManager.Instance.State == EGameState.GameOver) return;

        if (State == EBossState.Death) return;
        if (State == EBossState.Hit) return;
        if (_isJumping) return;

        _jumpCooldownTimer += Time.deltaTime;

        switch (State)
        {
            case EBossState.Idle:
                Idle();
                break;
            case EBossState.Walk:
                Walk();
                break;
            case EBossState.Attack:
                Attack();
                break;
        }
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (State == EBossState.Death) return false;

        bool isAlive = _bossStat.Health.Value - damage.Value > 0f;
        ChangeState(isAlive ? EBossState.Hit : EBossState.Death);
        return _bossCombat.TryTakeDamage(damage);
    }

    private void HandleHitComplete()
    {
        ChangeState(EBossState.Idle);
    }

    private void HandleDeath()
    {
        ChangeState(EBossState.Death);
        StartCoroutine(DeathDisappear_Coroutine());
    }

    private IEnumerator DeathDisappear_Coroutine()
    {
        yield return new WaitForSeconds(DEATH_DISAPPEAR_DELAY);
        gameObject.SetActive(false);
    }

    private void Idle()
    {
        if (_player == null) return;

        float distanceToPlayer = GetDistanceToPlayer();

        if (distanceToPlayer <= _bossStat.DetectRange.Value)
        {
            if (CanJump() && distanceToPlayer > _bossStat.AttackRange.Value)
            {
                StartJumpAttack();
            }
            else
            {
                ChangeState(EBossState.Walk);
            }
        }
    }

    private void Walk()
    {
        if (_player == null)
        {
            ChangeState(EBossState.Idle);
            return;
        }

        _bossMove.MoveTo(_player.transform.position);

        float distanceToPlayer = GetDistanceToPlayer();

        if (distanceToPlayer <= _bossStat.AttackRange.Value)
        {
            ChangeState(EBossState.Attack);
        }
        else if (CanJump() && distanceToPlayer > _bossStat.JumpRange.Value * 0.5f)
        {
            StartJumpAttack();
        }
    }

    private void Attack()
    {
        if (_player == null)
        {
            ChangeState(EBossState.Idle);
            return;
        }

        _bossMove.Stop();
        _bossMove.LookAt(_player.transform.position);

        if (GetDistanceToPlayer() > _bossStat.AttackRange.Value)
        {
            ChangeState(EBossState.Walk);
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _bossStat.AttackInterval.Value)
        {
            _attackTimer = 0f;
            _bossCombat.PerformAttack();
        }
    }

    private bool CanJump()
    {
        return _jumpCooldownTimer >= _bossStat.JumpCooldown.Value;
    }

    private void StartJumpAttack()
    {
        if (_player == null) return;

        _isJumping = true;
        _jumpCooldownTimer = 0f;
        _jumpTargetPosition = _player.transform.position;

        _bossMove.Stop();
        ChangeState(EBossState.JumpOut);
        StartCoroutine(JumpAttack_Coroutine());
    }

    private IEnumerator JumpAttack_Coroutine()
    {
        _animator.SetTrigger("JumpOut");
        _bossMove.SetAgentEnabled(false);

        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < JUMP_DURATION)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / JUMP_DURATION;

            Vector3 horizontalPos = Vector3.Lerp(startPosition, _jumpTargetPosition, t);
            float heightOffset = Mathf.Sin(t * Mathf.PI) * JUMP_HEIGHT;
            transform.position = new Vector3(horizontalPos.x, startPosition.y + heightOffset, horizontalPos.z);

            yield return null;
        }

        transform.position = new Vector3(_jumpTargetPosition.x, startPosition.y, _jumpTargetPosition.z);

        ChangeState(EBossState.JumpIn);
        _animator.SetTrigger("JumpIn");

        yield return new WaitForSeconds(0.5f);

        _bossMove.SetAgentEnabled(true);
        _bossMove.Warp(transform.position);
        _isJumping = false;

        ChangeState(EBossState.Attack);
    }

    private void ChangeState(EBossState newState)
    {
        if (State == newState) return;

        State = newState;

        switch (newState)
        {
            case EBossState.Idle:
                _bossMove.Stop();
                break;
            case EBossState.Walk:
                _animator.SetTrigger("Walk");
                break;
            case EBossState.Attack:
                break;
            case EBossState.JumpOut:
                break;
            case EBossState.JumpIn:
                break;
            case EBossState.Hit:
                break;
            case EBossState.Death:
                break;
        }
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _player.transform.position);
    }

    private Vector3 GetDirectionToPlayer()
    {
        return (_player.transform.position - transform.position).normalized;
    }

    public void DealDamageToPlayer()
    {
        if (State != EBossState.Attack && State != EBossState.JumpIn) return;
        if (_playerController == null) return;

        Damage damage = new Damage(
            _bossStat.AttackDamage.Value,
            GetDirectionToPlayer(),
            _player.transform.position,
            _bossStat.KnockbackForce.Value,
            Vector3.up
        );

        _playerController.TakeDamage(damage);
    }
}
