using UnityEngine;

public class MonsterAnimationEvent : MonoBehaviour
{
    [SerializeField] private Monster _monster;

    public void OnAttack()
    {
        _monster.DealDamageToPlayer();
    }

    public void OnDie()
    {
        Destroy(_monster.gameObject);
    }
}
