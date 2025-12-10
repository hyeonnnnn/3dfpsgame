using TMPro;
using UnityEngine;

public class UI_Projectile : MonoBehaviour
{
    [SerializeField] private TMP_Text _bombCount;
    [SerializeField] private PlayerBombFire _playerBombFire;

    [SerializeField] private TMP_Text _bulletCount;
    [SerializeField] private PlayerGunFire _playerGunFire;

    private void OnEnable()
    {
        _playerBombFire.OnBombCountChanged += UpdateBombCount;
        _playerGunFire.OnBulletCountChanged += UpdateBulletCount;
    }

    private void OnDisable()
    {
        _playerBombFire.OnBombCountChanged -= UpdateBombCount;
        _playerGunFire.OnBulletCountChanged -= UpdateBulletCount;
    }

    private void UpdateBombCount(int currentCount, int maxCount)
    {
        _bombCount.text = $"Bombs: {currentCount.ToString()} / {maxCount.ToString()}";
    }

    private void UpdateBulletCount(int currentCount, int remaingCount)
    {
        _bulletCount.text = $"Bullets: {currentCount.ToString()} / {remaingCount.ToString()}";
    }
}
