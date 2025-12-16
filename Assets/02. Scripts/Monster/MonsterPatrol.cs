using DG.Tweening;
using UnityEngine;

public enum EMonsterMoveType
{
    Idle,
    MoveForward,
    TurnLeft,
    TurnRight,
}

public class MonsterPatrol : MonoBehaviour
{
    [SerializeField] private float _patrolMaxInterval = 5f;
    [SerializeField] private float _patrolMinInterval = 2f;
    [SerializeField] private float _turnDuration = 0.4f;

    private MonsterMovement _movement;

    private float _patrolInterval;
    private float _patrolTimer = 0f;
    private EMonsterMoveType _currentMoveType = EMonsterMoveType.MoveForward;

    private void Awake()
    {
        _movement = GetComponent<MonsterMovement>();
    }

    public void UpdatePatrol()
    {
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
                _movement.MoveForward();
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
}
