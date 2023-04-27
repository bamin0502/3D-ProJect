using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    //동적 변수 선언
    public static string nextScene;
    //progress바를 직렬화함
    [SerializeField] Image ProgressBar;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void LoadScene(string SceneName)
    {
        nextScene = SceneName;
        SceneManager.LoadScene("Loading Scene");
    }
    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;
        float timer = 0.0f;
        //아직 처리가 완료되지 않았다면
        while (!operation.isDone)
        {
            //결과값을 반환함
            yield return null;
            timer += Time.deltaTime;
            //출력이 90퍼가 되기 전에는 
            if (operation.progress < 0.9f)
            {
                //점점 슬라이더를 채워감
                ProgressBar.fillAmount = Mathf.Lerp(ProgressBar.fillAmount, operation.progress, timer);
                if (ProgressBar.fillAmount >= operation.progress)
                {
                    timer = 0f;
                }

            }
            else
            {
                ProgressBar.fillAmount = Mathf.Lerp(ProgressBar.fillAmount, 1f, timer);
                //진행도가 1이 되었으면
                if (ProgressBar.fillAmount == 1.0f)
                {
                    //씬이동을 시키고
                    operation.allowSceneActivation = true;
                    
                    //종료시킴
                    yield break;
                }
            }
        }
    }
}
