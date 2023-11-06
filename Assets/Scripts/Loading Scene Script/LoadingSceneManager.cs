using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Cysharp.Threading.Tasks;
public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
    [SerializeField] Image ProgressBar;
    [SerializeField] Image OtherImage;
    [SerializeField] float fillSpeed = 0.1f;
    [SerializeField] float otherImageFillSpeed = 0.2f;
    private bool fillingOtherImage = true;
    void Start()
    {
        LoadSceneAsync().Forget();
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading Scene");
    }
    async UniTaskVoid LoadSceneAsync()
    {
        await UniTask.Yield(); // 첫 프레임 스킵

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;
        float timer = 0.0f;
        float targetFillAmount = 0.9f; // 목표로 하는 fillAmount 값 (90%)
        float timeToFill = 5.0f; // ProgressBar를 채우는 데 걸리는 시간

        while (!operation.isDone)
        {
            await UniTask.Yield();

            timer += Time.deltaTime;

            // ProgressBar를 천천히 차게 보이도록 보간 처리
            float progress = Mathf.Lerp(ProgressBar.fillAmount, operation.progress, timer / timeToFill);
            ProgressBar.fillAmount = progress;

            OtherImage.fillAmount = Mathf.Clamp01(OtherImage.fillAmount + (fillSpeed * Time.deltaTime));
            if (ProgressBar.fillAmount >= targetFillAmount)
            {
                // ProgressBar가 목표치에 도달하면 나머지 시간을 기다린 후 씬 전환
                if (timer >= timeToFill)
                {
                    operation.allowSceneActivation = true;
                    break;
                }
                if (fillingOtherImage)
                {
                    OtherImage.fillAmount += otherImageFillSpeed * Time.deltaTime;
                    // OtherImage가 1에 도달하면 빠르게 비우도록 상태 변경
                    if (OtherImage.fillAmount >= 1.0f)
                    {
                        fillingOtherImage = false;
                        OtherImage.fillAmount = 1.0f; // 최대로 채운 후 빠르게 비우도록
                    }
                }
                else
                {
                    OtherImage.fillAmount -= otherImageFillSpeed * Time.deltaTime;
                    // OtherImage가 0에 도달하면 다시 채우도록 상태 변경
                    if (OtherImage.fillAmount <= 0.0f)
                    {
                        fillingOtherImage = true;
                        OtherImage.fillAmount = 0.0f; // 최저로 비운 후 빠르게 채우도록
                    }
                }
            }
        }
    }
}