using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [field: SerializeField] public ConsumableStat Stamina { get; private set; }
    [field: SerializeField] public ConsumableStat Health { get; private set; }

    [field: SerializeField] public ValueStat MoveSpeed { get; private set; }
    [field: SerializeField] public ValueStat RunSpeed { get; private set; }
    [field: SerializeField] public ValueStat WalkSpeed { get; private set; }
    [field: SerializeField] public ValueStat JumpForce { get; private set; }

    [field: SerializeField] public ValueStat Damage { get; private set; }

    private void Start()
    {
        Stamina.Initialize();
        Health.Initialize();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        Stamina.Regenerate(deltaTime);
        Health.Regenerate(deltaTime);
    }
}
