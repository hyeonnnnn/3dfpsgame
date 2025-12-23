using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScene : MonoBehaviour
{
    private enum SceneMode
    {
        Login,
        Register
    }

    private SceneMode _mode = SceneMode.Login;

    [SerializeField] private GameObject _passwordConfirmObjectUI;

    [SerializeField] private Button _loginButtonUI;
    [SerializeField] private Button _gotoLoginButtonUI;
    [SerializeField] private Button _registerButtonUI;
    [SerializeField] private Button _gotoRegisterButtonUI;

    [SerializeField] private TMP_InputField _idInputFieldUI;
    [SerializeField] private TMP_InputField _passwordInputFieldUI;
    [SerializeField] private TMP_InputField _passwordConfirmInputFieldUI;

    [SerializeField] private TextMeshProUGUI _messageTextUI;

    private void Start()
    {
        AddButtonEvent();
        Refresh();
    }

    private void AddButtonEvent()
    {
        _loginButtonUI.onClick.AddListener(Login);
        _gotoRegisterButtonUI.onClick.AddListener(GotoRegister);
        _registerButtonUI.onClick.AddListener(Register);
        _gotoLoginButtonUI.onClick.AddListener(GotoLogin);
    }


    private void Refresh()
    {
        _passwordConfirmObjectUI.SetActive(_mode == SceneMode.Register);

        _loginButtonUI.gameObject.SetActive(_mode == SceneMode.Login);
        _gotoRegisterButtonUI.gameObject.SetActive(_mode == SceneMode.Login);

        _registerButtonUI.gameObject.SetActive(_mode == SceneMode.Register);
        _gotoLoginButtonUI.gameObject.SetActive(_mode == SceneMode.Register);
    }

    private void Login()
    {
        // 아이디 입력 확인
        string id = _idInputFieldUI.text;
        if(string.IsNullOrEmpty(id))
        {
            _messageTextUI.text = "아이디를 입력해주세요.";
            return;
        }

        // 비밀번호 입력 확인
        string password = _passwordInputFieldUI.text;
        if(string.IsNullOrEmpty(password))
        {
            _messageTextUI.text = "비밀번호를 입력해주세요.";
            return;
        }

        // 실제 저장된 아이디, 비밀번호와 비교 후 로그인 처리
        // 아이디가 있는지 확인한다.
        if (PlayerPrefs.HasKey(id) == false)
        {
            _messageTextUI.text = "아이디/비밀번호를 확인해주세요.";
            return;
        }

        string savedPassword = PlayerPrefs.GetString(id);
        if (savedPassword != password)
        {
            _messageTextUI.text = "아이디/비밀번호를 확인해주세요.";
            return;
        }

        // 로그인 성공
        SceneManager.LoadScene("LoadingScene");
    }

    private void Register()
    {
        string id = _idInputFieldUI.text;
        if (string.IsNullOrEmpty(id))
        {
            _messageTextUI.text = "아이디를 입력해주세요.";
            return;
        }

        string password = _passwordInputFieldUI.text;
        if (string.IsNullOrEmpty(password))
        {
            _messageTextUI.text = "비밀번호를 입력해주세요.";
            return;
        }

        string password2 = _passwordInputFieldUI.text;
        if (string.IsNullOrEmpty(password))
        {
            _messageTextUI.text = "아이디/비밀번호를 확인해주세요.";
            return;
        }

        if (PlayerPrefs.HasKey(id) == true)
        {
            _messageTextUI.text = "중복된 아이디입니다.";
            return;
        }
        PlayerPrefs.SetString(id, password);
        GotoLogin();
    }

    private void GotoLogin()
    {
        _mode = SceneMode.Login;
        Refresh();
    }

    private void GotoRegister()
    {
        _mode = SceneMode.Register;
        Refresh();
    }
}
