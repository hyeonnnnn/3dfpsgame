using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_OptionPopup : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _RetryButton;
    [SerializeField] private Button _ExitButton;

    public void Show()
    {
        Debug.Log("Show Option Popup");
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        Debug.Log("Hide Option Popup");
        gameObject.SetActive(false);
    }

    private void Start()
    {
        _continueButton.onClick.AddListener(GameContinue);
        _RetryButton.onClick.AddListener(GameRetry);
        _ExitButton.onClick.AddListener(GameExit);
    }

    private void GameContinue()
    {
        Time.timeScale = 1f;
        Hide();
    }

    private void GameRetry()
    {
        Time.timeScale = 1f;
        GameManager.Instance.Restart();
    }

    private void GameExit()
    {
        GameManager.Instance.Quit();
    }
}
