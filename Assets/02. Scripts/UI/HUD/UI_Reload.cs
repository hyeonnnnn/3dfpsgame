using UnityEngine;
using UnityEngine.UI;

public class UI_Reload : MonoBehaviour
{
    [SerializeField] private AmmoController _ammoController;
    private Image _reloadImageUI;

    private float _reloadCoolTime;
    private float _fillAmount = 0f;
    private bool _isReloading = false;

    private void Awake()
    {
        _reloadImageUI = GetComponent<Image>();
        _reloadImageUI.fillAmount = 0f;
    }

    private void Update()
    {
        if (_isReloading)
        {
            _fillAmount += Time.deltaTime / _reloadCoolTime;
            _reloadImageUI.fillAmount = _fillAmount;
            
            if (_fillAmount >= 1f)
            {
                _isReloading = false;
                _fillAmount = 0f;
                _reloadImageUI.fillAmount = 0f;
            }
        }
    }

    private void OnEnable()
    {
        _ammoController.OnReloaded += HandleReloaded;
    }
    private void OnDisable()
    {
        _ammoController.OnReloaded -= HandleReloaded;
    }

    private void HandleReloaded(float reloadTime)
    {
        _isReloading = true;
        _reloadCoolTime = reloadTime;
    }
}
