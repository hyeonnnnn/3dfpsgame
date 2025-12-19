using UnityEngine;

public class MonsterAnimationEvent : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private MonsterStat _stats;

    public void OnAttack()
    {
        if (_monster.State != EMonsterState.Attack) return;

        PlayerController playerController = _monster.GetPlayerController();
        if (playerController == null) return;

        Vector3 direction = _monster.GetDirectionToPlayer();
        Vector3 hitPoint = playerController.transform.position;
        Damage damage = new Damage(_stats.AttackDamage.Value, direction, hitPoint, _stats.KnockbackForce.Value, Vector3.up);
        playerController.TakeDamage(damage);
    }

    public void OnDie()
    {
        Destroy(_monsterPrefab);
    }
}
