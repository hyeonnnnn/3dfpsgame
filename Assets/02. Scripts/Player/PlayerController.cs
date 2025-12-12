using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    private PlayerStats _playerStats;
    private CharacterController _controller;

    [SerializeField] private float _knockbackDuration = 0.2f;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _controller = GetComponent<CharacterController>();
    }

    public void TryTakeDamage(Damage damage)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(Damage damage)
    {
        _playerStats.Health.Decrease(damage.Value);
        StartCoroutine(Hit_Coroutine(damage.Direction, damage.KnockbackForce));

        if (_playerStats.Health.Value <= 0f)
        {
            Die();
        }
    }

    private IEnumerator Hit_Coroutine(Vector3 direction, float knockbackForce)
    {
        float elapsed = 0f;
        Vector3 knockbackVelocity = direction * knockbackForce;

        while (elapsed < _knockbackDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / _knockbackDuration;
            Vector3 velocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, progress);

            _controller.Move(velocity * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
