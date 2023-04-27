using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    //���� ���� ����
    public static string nextScene;
    //progress�ٸ� ����ȭ��
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
        //���� ó���� �Ϸ���� �ʾҴٸ�
        while (!operation.isDone)
        {
            //������� ��ȯ��
            yield return null;
            timer += Time.deltaTime;
            //����� 90�۰� �Ǳ� ������ 
            if (operation.progress < 0.9f)
            {
                //���� �����̴��� ä����
                ProgressBar.fillAmount = Mathf.Lerp(ProgressBar.fillAmount, operation.progress, timer);
                if (ProgressBar.fillAmount >= operation.progress)
                {
                    timer = 0f;
                }

            }
            else
            {
                ProgressBar.fillAmount = Mathf.Lerp(ProgressBar.fillAmount, 1f, timer);
                //���൵�� 1�� �Ǿ�����
                if (ProgressBar.fillAmount == 1.0f)
                {
                    //���̵��� ��Ű��
                    operation.allowSceneActivation = true;
                    
                    //�����Ŵ
                    yield break;
                }
            }
        }
    }
}
