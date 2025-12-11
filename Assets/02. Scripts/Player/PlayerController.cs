using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    private PlayerStats _playerStats;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    public void TakeDamage(float damage)
    {
        _playerStats.Health.Consume(damage);
        if (_playerStats.Health.Value <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
