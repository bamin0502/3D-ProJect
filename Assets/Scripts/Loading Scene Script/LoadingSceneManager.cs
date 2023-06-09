using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
    [SerializeField] Image ProgressBar;
    [SerializeField] TMP_Text ProgressText;
    [SerializeField] string[] sentences;
    [SerializeField] float textDelay = 3.0f;

    private int currentSentenceIndex = 0;

    void Start()
    {
        StartCoroutine(LoadScene());
        StartCoroutine(DisPlaySentences());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading Scene");
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;
        float timer = 0.0f;
        float targetFillAmount = 0.9f; // 목표로 하는 fillAmount 값 (90%)
        float timeToFill = 5.0f; // ProgressBar를 채우는 데 걸리는 시간

        while (!operation.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            // ProgressBar를 천천히 차게 보이도록 보간 처리
            float progress = Mathf.Lerp(ProgressBar.fillAmount, operation.progress, timer / timeToFill);
            ProgressBar.fillAmount = progress;

            if (ProgressBar.fillAmount >= targetFillAmount)
            {
                // ProgressBar가 목표치에 도달하면 나머지 시간을 기다린 후 씬 전환
                if (timer >= timeToFill)
                {
                    operation.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
    IEnumerator DisPlaySentences()
    {
        while (currentSentenceIndex < sentences.Length)
        {
            int randomIndex = Random.Range(0, sentences.Length);
            ProgressText.text = sentences[randomIndex];
            currentSentenceIndex++;
            yield return new WaitForSeconds(textDelay);
        }
    }
}
