using TMPro;
using UnityEngine;

public class UI_Projectile : MonoBehaviour
{
    [SerializeField] private TMP_Text _bombCount;
    [SerializeField] private PlayerBombFire _playerBombFire;

    [SerializeField] private TMP_Text _magazineCount;
    [SerializeField] private AmmoController _ammoController;

    private void OnEnable()
    {
        _playerBombFire.OnBombCountChanged += UpdateBombCount;
        _ammoController.OnAmmoCountChanged += UpdateMagazineCount;
    }

    private void OnDisable()
    {
        _playerBombFire.OnBombCountChanged -= UpdateBombCount;
        _ammoController.OnAmmoCountChanged -= UpdateMagazineCount;
    }

    private void UpdateBombCount(int currentCount, int maxCount)
    {
        _bombCount.text = $"Bombs: {currentCount} / {maxCount}";
    }

    private void UpdateMagazineCount(int currentCount, int remaingCount)
    {
        _magazineCount.text = $"Bullets: {currentCount} / {remaingCount}";
    }
}
