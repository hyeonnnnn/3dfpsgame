using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Slider _progressSliderUI;
    [SerializeField] private TextMeshProUGUI _progressTextUI;

    private void Start()
    {
        StartCoroutine(LoadScene_Coroutine());
    }

    private IEnumerator LoadScene_Coroutine()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync("GameScene");

        // 로드되는 씬 모습이 보이지 않도록 설정
        ao.allowSceneActivation = false;

        // 로드가 완료될 때까지 진행
        while (!ao.isDone)
        {
            _progressSliderUI.value = ao.progress;
            _progressTextUI.text = $"{(ao.progress * 100f)}%";

            if (ao.progress >= 0.9f)
            {
                _progressSliderUI.value = 1f;
                _progressTextUI.text = "100%";
                ao.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
