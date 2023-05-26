using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data.Common;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class GameManager : SerializedMonoBehaviour
{
    public static GameManager Inst;
    [Title ("관리 오브젝트 설정")]
    //리스트배열 형태로 이미지를 관리함
    [SerializeField]
    private GameObject MainMenuImage;
    [SerializeField]
    private GameObject MainOptionImage;
    [SerializeField]
    private GameObject NoticeImage;
    [SerializeField]
    private GameObject SourceImage;
    [SerializeField]
    private GameObject GameOptionImage;


    private void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //옵션 창 비활성화
        MainOptionImage.gameObject.SetActive(false);
        //경고 창 비활성화
        NoticeImage.gameObject.SetActive(false);
        //출처 표시 창 비활성화
        SourceImage.gameObject.SetActive(false);
    }
    //게임 실행을 시킬 메서드 지정
    public void GamestartButtonClick()
    {
        //로딩씬이 실행후에 게임씬으로 이동하도록 처리
        LoadingSceneManager.LoadScene("Game Scene");
    }
    //게임 종료를 시킬 메서드 지정
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료 확인용");
    }
    public void GitButtonClicked()
    {
        Application.OpenURL("https://github.com/bamin0502/3D-ProJect");
    }
    public void YouTubeButtonClicked()
    {
        //뒤에 영상 작업하고 올릴링크 지정
        Application.OpenURL("");
    }
    public void BackMainMenu()
    {
        LoadingSceneManager.LoadScene("Start Menu Scene");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MainMenuImage = GameObject.FindGameObjectWithTag("Start Menu");
        if (MainMenuImage == null)
        {
            Debug.Log("해당 오브젝트를 찾을 수 없습니다.");
        }

        MainOptionImage = GameObject.FindGameObjectWithTag("Menu Option");
        if (MainOptionImage == null)
        {
            Debug.Log("해당 오브젝트를 찾을 수 없습니다.");
        }
        else
        {
            MainOptionImage.gameObject.SetActive(false);
        }

        NoticeImage = GameObject.FindGameObjectWithTag("Notice");
        if (NoticeImage == null)
        {
            Debug.Log("해당 오브젝트를 찾을 수 없습니다.");
        }
        else
        {
            NoticeImage.gameObject.SetActive(false);
        }

        SourceImage = GameObject.FindGameObjectWithTag("Source");
        if (SourceImage == null)
        {
            Debug.Log("해당 오브젝트를 찾을 수 없습니다.");
        }
        else
        {
            SourceImage.gameObject.SetActive(false);
        }

        GameOptionImage = GameObject.FindWithTag("Game Option");
        if (GameOptionImage == null)
        {
            Debug.Log("해당 오브젝트를 찾을 수 없습니다.");
        }
        else
        {
            GameOptionImage.gameObject.SetActive(false);
        }

        if (scene.name == "Start Menu Scene")
        {
            Button singleButton = MainMenuImage.transform.Find("SingleMode").GetComponent<Button>();
            if (singleButton != null)
            {
                singleButton.onClick.AddListener(GamestartButtonClick);
            }
            else
            {
                Debug.Log("해당 오브젝트를 찾을 수 없습니다.");
            }

            Button quitButton = MainMenuImage.transform.Find("QuitButton").GetComponent<Button>();
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }
            else
            {
                Debug.Log("해당 오브젝트를 찾을 수 없습니다.");
            }

            Button gitButton = MainOptionImage.transform.Find("GitButton").GetComponent<Button>();
            if (gitButton != null)
            {
                gitButton.onClick.AddListener(GitButtonClicked);
            }
            else
            {
                Debug.Log("해당 오브젝트를 찾을 수 없습니다.");
            }

            Button youButton = MainOptionImage.transform.Find("YoutubeButton").GetComponent<Button>();
            if (youButton != null)
            {
                youButton.onClick.AddListener(YouTubeButtonClicked);
            }
            else
            {
                Debug.Log("해당 오브젝트를 찾을 수 없습니다.");
            }
            }
            else if (scene.name == "Game Scene")
            {
            Button homeButton = GameOptionImage.transform.Find("HomeButton").GetComponent<Button>();
            if (homeButton != null)
            {
                homeButton.onClick.AddListener(BackMainMenu);
            }
            else
            {
                Debug.Log("해당 오브젝트를 찾을 수 없습니다.");
            }
        }
    }
    
}
