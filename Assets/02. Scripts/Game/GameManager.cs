using System;
using System.Collections;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    
    [SerializeField] private PlayerController _playerController;

    private const float ReadyDuration = 2f;

    public Action<EGameState> OnGameStateChange;

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

    private void OnDisable()
    {
        if (_playerController != null)
            _playerController.OnPlayerDeath -= GameOver;
    }

    private void Start()
    {
        if (_playerController != null)
            _playerController.OnPlayerDeath += GameOver;

        _state = EGameState.Ready;
        OnGameStateChange?.Invoke(_state);

        StartCoroutine(GameStart_Coroutine());
    }

    private IEnumerator GameStart_Coroutine()
    {
        yield return new WaitForSeconds(ReadyDuration);

        _state = EGameState.Playing;
        OnGameStateChange?.Invoke(_state);
    }

    private void GameOver()
    {
        _state = EGameState.GameOver;
        OnGameStateChange?.Invoke(_state);
    }

}
