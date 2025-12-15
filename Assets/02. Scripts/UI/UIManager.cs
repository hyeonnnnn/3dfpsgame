using TMPro;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    [SerializeField] private TextMeshProUGUI _stateTextUI;

    private const float StartDuration = 1f;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChange += UpdateGameStateText;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChange -= UpdateGameStateText;
    }

    private void UpdateGameStateText(EGameState state)
    {
        switch (state)
        {
            case EGameState.Ready:
                _stateTextUI.text = "준비 중";
                break;

            case EGameState.Playing:
                StartCoroutine(StartToPlay_Coroutine());
                break;

            case EGameState.GameOver:
                _stateTextUI.text = "게임 오버";
                _stateTextUI.gameObject.SetActive(true);
                break;

        }
    }

    private IEnumerator StartToPlay_Coroutine()
    {
        _stateTextUI.text = "시작!";

        yield return new WaitForSeconds(StartDuration);

        _stateTextUI.gameObject.SetActive(false);
    }
}
