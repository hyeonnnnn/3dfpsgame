using UnityEngine;

public class MonsterStats : MonoBehaviour
{
    [field: SerializeField] public ConsumableStat Health { get; private set; }
    [field: SerializeField] public ValueStat MoveSpeed { get; private set; }
    [field: SerializeField] public ValueStat DetectRange { get; private set; }
    [field: SerializeField] public ValueStat TraceRange { get; private set; }
    [field: SerializeField] public ValueStat AttackRange { get; private set; }
    [field: SerializeField] public ValueStat AttackInterval { get; private set; }
    [field: SerializeField] public ValueStat AttackDamage { get; private set; }
    [field: SerializeField] public ValueStat KnockbackDuration { get; private set; }
    [field: SerializeField] public ValueStat KnockbackForce { get; private set; }

    private void Start()
    {
        Health.Initialize();
    }
}
