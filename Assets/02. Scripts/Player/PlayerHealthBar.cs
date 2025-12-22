using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private PlayerStats _stat;
    private PlayerController _playerController;

    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _guageImage;

    private float _lastHealth = -1;

    private void Awake()
    {
        _stat = GetComponent<PlayerStats>();
        _playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        _playerController.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        _playerController.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        if (_lastHealth != _stat.Health.Value)
        {
            _lastHealth = _stat.Health.Value;
            _guageImage.fillAmount = _stat.Health.Value / _stat.Health.MaxValue;
        }
    }

    private void LateUpdate()
    {
        _healthBarTransform.forward = Camera.main.transform.forward;
    }
}
