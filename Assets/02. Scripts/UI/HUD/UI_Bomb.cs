using TMPro;
using UnityEngine;

public class UI_Bomb : MonoBehaviour
{
    [SerializeField] private TMP_Text _bombCount;
    [SerializeField] private PlayerBombFire _playerFire;

    private void OnEnable()
    {
        _playerFire.OnBombCountChanged += UpdateBombCount;
    }

    private void OnDisable()
    {
        _playerFire.OnBombCountChanged -= UpdateBombCount;
    }

    private void UpdateBombCount(int count)
    {
        _bombCount.text = $"Bombs: {count.ToString()}";
    }
}
