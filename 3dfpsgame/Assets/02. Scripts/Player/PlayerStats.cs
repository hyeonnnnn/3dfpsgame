using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public ConsumableStat Stamina;
    public ConsumableStat Health;

    public ValueStat MoveSpeed;
    public ValueStat RunSpeed;
    public ValueStat WalkSpeed;
    public ValueStat JumpForce;

    public ValueStat Damage;

    private void Start()
    {
        Stamina.Initialize();
        Health.Initialize();
    }

    private void Update()
    {
        Stamina.Regenerate(Time.deltaTime);
        Health.Regenerate(Time.deltaTime);
    }
}
