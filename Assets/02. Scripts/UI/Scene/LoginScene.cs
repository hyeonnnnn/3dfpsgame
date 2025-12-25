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

    private const string AllowedSpecials = "!@#$%^&*()_+-=[]{}|;:'\",.<>/?`~\\";

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
        string id = _idInputFieldUI.text;
        if (string.IsNullOrEmpty(id))
        {
            _messageTextUI.text = "아이디를 입력해주세요.";
            return;
        }

        if (IsEmailType(id) == false)
        {
            _messageTextUI.text = "올바른 이메일 형식이 아닙니다.";
            return;
        }

        string password = _passwordInputFieldUI.text;
        if(string.IsNullOrEmpty(password))
        {
            _messageTextUI.text = "비밀번호를 입력해주세요.";
            return;
        }

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

        SceneManager.LoadScene(SceneNames.LoadingScene);
    }

    private void Register()
    {
        string id = _idInputFieldUI.text;
        if (string.IsNullOrEmpty(id))
        {
            _messageTextUI.text = "아이디를 입력해주세요.";
            return;
        }

        if (IsEmailType(id) == false)
        {
            _messageTextUI.text = "올바른 이메일 형식이 아닙니다.";
            return;
        }

        string password = _passwordInputFieldUI.text;
        if (string.IsNullOrEmpty(password))
        {
            _messageTextUI.text = "비밀번호를 입력해주세요.";
            return;
        }

        string password2 = _passwordConfirmInputFieldUI.text;
        if (string.IsNullOrEmpty(password2))
        {
            _messageTextUI.text = "아이디/비밀번호를 확인해주세요.";
            return;
        }

        if (password != password2)
        {
            _messageTextUI.text = "아이디/비밀번호를 확인해주세요.";
            return;
        }

        if (PlayerPrefs.HasKey(id) == true)
        {
            _messageTextUI.text = "중복된 아이디입니다.";
            return;
        }

        if (TryValidate(password, out string message) == false)
        {
            _messageTextUI.text = message;
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

    private bool IsEmailType(string id)
    {
        return id.Contains("@") && id.Contains(".");
    }

    public static bool TryValidate(string password, out string message)
    {
        if (IsLengthValid(password) == false)
        {
            message = "비밀번호는 7자 이상 20자 이하로 설정해야 합니다.";
            return false;
        }

        if (HasSpecialAtLeastOne(password) == false)
        {
            message = "비밀번호는 특수문자를 최소 1개 이상 포함해야 합니다.";
            return false;
        }

        if (HasUpperAtLeastOne(password) == false || HasLowerAtLeastOne(password) == false)
        {
            message = "비밀번호는 대소문자를 최소 1개 이상 포함해야 합니다.";
            return false;
        }

        if (IsOnlyAllowedChars(password) == false)
        {
            message = "비밀번호는 영어/숫자/특수문자만 가능합니다.";
            return false;
        }

        message = "회원가입이 완료되었습니다.";
        return true;
    }

    private static bool IsLengthValid(string password)
    {
        return password.Length >= 7 && password.Length <= 20;
    }

    private static bool HasSpecialAtLeastOne(string password)
    {
        foreach (char c in password)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return true;
            }
        }
        return false;
    }

    private static bool HasUpperAtLeastOne(string password)
    {
        foreach (char c in password)
        {
            if (char.IsUpper(c))
            {
                return true;
            }
        }
        return false;
    }

    private static bool HasLowerAtLeastOne(string password)
    {
        foreach (char c in password)
        {
            if (char.IsLower(c))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsOnlyAllowedChars(string password)
    {
        foreach (char c in password)
        {
            bool isLetter = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
            bool isDigit = (c >= '0' && c <= '9');
            bool isSpecial = AllowedSpecials.IndexOf(c) >= 0;

            if (!isLetter && !isDigit && !isSpecial)
            {
                return false;
            }
        }
        return true;
    }
}
