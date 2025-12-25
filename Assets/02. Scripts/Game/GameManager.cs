using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private UI_OptionPopup _optionPopupUI;
  

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
            _optionPopupUI.Show();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        // UnlockCursor();
    }

    public void Continue()
    {
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LoadingScene");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
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
