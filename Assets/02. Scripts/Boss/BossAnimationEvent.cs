using UnityEngine;

public class BossAnimationEvent : MonoBehaviour
{
    private Boss _boss;

    private void Awake()
    {
        _boss = GetComponentInParent<Boss>();
    }

    public void OnAttackHit()
    {
        if (_boss != null)
        {
            _boss.DealDamageToPlayer();
        }
    }

    public void OnJumpLand()
    {
        if (_boss != null)
        {
            _boss.DealDamageToPlayer();
        }
    }
}
