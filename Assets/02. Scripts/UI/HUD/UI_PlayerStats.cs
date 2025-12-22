using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;

    private void Start()
    {
        Refresh();
    }

    private void OnEnable()
    {
        PlayerStats.OnDataChanged += Refresh;
    }

    private void Refresh()
    {
        _healthSlider.value = _stats.Health.Value / _stats.Health.MaxValue;
        _staminaSlider.value = _stats.Stamina.Value / _stats.Stamina.MaxValue;
    }

    private void OnDestroy()
    {
        PlayerStats.OnDataChanged -= Refresh;
    }
}
