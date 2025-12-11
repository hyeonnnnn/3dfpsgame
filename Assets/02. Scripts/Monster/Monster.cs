using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public EMonsterState State = EMonsterState.Idle;

    [SerializeField] private float _health = 100f;
    [SerializeField] private float _detectRange = 4f;
    [SerializeField] private float _attackRange = 1.2f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _attackSpeed = 2;
    private float _attackTimer = 0f;

    private GameObject _player;
    private CharacterController _controller;

    private Vector3 _originPosition;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _originPosition = transform.position;
    }

    private void Update()
    {
        switch(State)
        {
            case EMonsterState.Idle:
                Idle();
                break;

            case EMonsterState.Trace:
                Trace();
                break;

            case EMonsterState.Comeback:
                Comeback();
                break;

            case EMonsterState.Attack:
                Attack();
                break;
        }
    }

    private void Idle()
    {
        // Todo. Idle 애니메이션 재생
        if (Vector3.Distance(transform.position, _player.transform.position) <= _detectRange)
        {
            State = EMonsterState.Trace;
            Debug.Log("Idle -> Trace");
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
            Debug.Log("Trace -> Idle");
        }

        if (distance <= _attackRange)
        {
            State = EMonsterState.Attack;
            Debug.Log("Trace -> Attack");
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
        if (_attackTimer >= _attackSpeed)
        {
            _attackTimer = 0f;
            Debug.Log("Attack!");
            // 과제. 플레이어 공격 처리
        }
    }

    public bool TryTakeDamage(float damage)
    {
        if (State == EMonsterState.Death) return false;

        _health -= damage;

        if (_health > 0f)
        {
            State = EMonsterState.Hit;
            StartCoroutine(Hit_Coroutine());
        }
        else
        {
            State = EMonsterState.Death;
            StartCoroutine(Death_Coroutine());
        }
        return true;
    }

    private IEnumerator Hit_Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        State = EMonsterState.Idle;
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
