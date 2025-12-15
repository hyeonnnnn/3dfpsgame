using System.Collections;
using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    [SerializeField] private TextMeshProUGUI _stateTextUI;
    [SerializeField] private PlayerController _playerController;

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
        InitState();
        StartCoroutine(StartToPlay_Coroutine());

        _playerController.OnPlayerDeath += GameOver;
    }

    private void InitState()
    {
        _state = EGameState.Ready;
        _stateTextUI.text = "준비 중..";
    }

    private IEnumerator StartToPlay_Coroutine()
    {
        yield return new WaitForSeconds(2f);

        _stateTextUI.text = "시작!";
        _state = EGameState.Playing;
        
        _stateTextUI.gameObject.SetActive(false);
    }

    private void GameOver()
    {
        _stateTextUI.gameObject.SetActive(true);
        _state = EGameState.GameOver;
        _stateTextUI.text = "게임 오버..";
    }
}
