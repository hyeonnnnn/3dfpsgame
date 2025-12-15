using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MonsterStat))]
public class MonsterHeathBar : MonoBehaviour
{
    private MonsterStat _stat;
    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _guageImage;

    private float _lastHealth = -1;

    private void Awake()
    {
        _stat = GetComponent<MonsterStat>();
    }

    private void LateUpdate()
    {
        if (_lastHealth != _stat.Health.Value)
        {
            _lastHealth = _stat.Health.Value;
            _guageImage.fillAmount = _stat.Health.Value / _stat.Health.MaxValue;
        }

        _healthBarTransform.forward = Camera.main.transform.forward;
    }
}
