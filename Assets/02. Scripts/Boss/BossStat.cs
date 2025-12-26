using UnityEngine;
using System;

public class BossStat : MonoBehaviour
{
    [field: SerializeField] public ConsumableStat Health { get; private set; }
    [field: SerializeField] public ValueStat MoveSpeed { get; private set; }
    [field: SerializeField] public ValueStat DetectRange { get; private set; }
    [field: SerializeField] public ValueStat AttackRange { get; private set; }
    [field: SerializeField] public ValueStat AttackInterval { get; private set; }
    [field: SerializeField] public ValueStat AttackDamage { get; private set; }
    [field: SerializeField] public ValueStat KnockbackForce { get; private set; }
    [field: SerializeField] public ValueStat AngularSpeed { get; private set; }
    [field: SerializeField] public ValueStat JumpRange { get; private set; }
    [field: SerializeField] public ValueStat JumpCooldown { get; private set; }

    public event Action OnHealthChanged;

    private void Start()
    {
        Health.Initialize(NotifyHealthChanged);
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke();
    }
}
