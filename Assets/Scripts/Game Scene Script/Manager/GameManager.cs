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
    [SerializeField]
    private GameObject EndingImage;
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
    //멀티 게임 실행을 시킬 메서드 지정(일단은 작업을 위해 이동은 시키나 나중에는 로비씬에서 이동시킬거임)
    public void MultiGameStartButtonClick()
    {
        LoadingSceneManager.LoadScene("Multi Scene");
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
        Application.OpenURL("https://youtu.be/yMu0XA16A-E");
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
            Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
        }

        MainOptionImage = GameObject.FindGameObjectWithTag("Menu Option");
        if (MainOptionImage == null)
        {
            Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
        }
        else
        {
            MainOptionImage.gameObject.SetActive(false);
        }

        NoticeImage = GameObject.FindGameObjectWithTag("Notice");
        if (NoticeImage == null)
        {
            Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
        }
        else
        {
            NoticeImage.gameObject.SetActive(false);
        }

        SourceImage = GameObject.FindGameObjectWithTag("Source");
        if (SourceImage == null)
        {
            Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
        }
        else
        {
            SourceImage.gameObject.SetActive(false);
        }

        GameOptionImage = GameObject.FindWithTag("Game Option");
        if (GameOptionImage == null)
        {
            Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
        }
        else
        {
            GameOptionImage.gameObject.SetActive(false);
        }
        EndingImage = GameObject.FindGameObjectWithTag("Ending Image");
        if(EndingImage == null)
        {
            Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
        }
        else
        {
            EndingImage.gameObject.SetActive(false);
        }
        //시작메뉴 씬 관련 오브젝트
        if (scene.name == "Start Menu Scene")
        {
            Button singleButton = MainMenuImage.transform.Find("SingleMode").GetComponent<Button>();
            if (singleButton != null)
            {
                singleButton.onClick.AddListener(GamestartButtonClick);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }
            Button multiButton = MainMenuImage.transform.Find("Multi Text").GetComponent<Button>();
            if(multiButton != null)
            {
                multiButton.onClick.AddListener(MultiGameStartButtonClick);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }           
            Button quitButton = MainMenuImage.transform.Find("QuitButton").GetComponent<Button>();
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }

            Button gitButton = MainOptionImage.transform.Find("GitButton").GetComponent<Button>();
            if (gitButton != null)
            {
                gitButton.onClick.AddListener(GitButtonClicked);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }

            Button youButton = MainOptionImage.transform.Find("YoutubeButton").GetComponent<Button>();
            if (youButton != null)
            {
                youButton.onClick.AddListener(YouTubeButtonClicked);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }
        }
        //게임 씬 오브젝트 
        else if (scene.name == "Game Scene")
        {

            Button homeButton = GameOptionImage.transform.Find("HomeButton").GetComponent<Button>();
            if (homeButton != null)
            {
                homeButton.onClick.AddListener(BackMainMenu);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }
            
            Button RestartButton = EndingImage.transform.Find("Home Button").GetComponent<Button>();
            if(RestartButton != null)
            {
                RestartButton.onClick.AddListener(GamestartButtonClick);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }
            Button MainButton = EndingImage.transform.Find("Main Button").GetComponent<Button>();
            if(MainButton != null)
            {
                MainButton.onClick.AddListener(BackMainMenu);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }
            Button QuitButton = EndingImage.transform.Find("Quit Button").GetComponent<Button>();
            if(QuitButton != null)
            {
                QuitButton.onClick.AddListener(QuitGame);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }

        }
        //게임 씬 오브젝트 
        else if (scene.name == "Multi Scene")
        {

            Button homeButton = GameOptionImage.transform.Find("HomeButton").GetComponent<Button>();
            if (homeButton != null)
            {
                homeButton.onClick.AddListener(BackMainMenu);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }

            Button RestartButton = EndingImage.transform.Find("Home Button").GetComponent<Button>();
            if (RestartButton != null)
            {
                RestartButton.onClick.AddListener(GamestartButtonClick);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }
            Button MainButton = EndingImage.transform.Find("Main Button").GetComponent<Button>();
            if (MainButton != null)
            {
                MainButton.onClick.AddListener(BackMainMenu);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }
            Button QuitButton = EndingImage.transform.Find("Quit Button").GetComponent<Button>();
            if (QuitButton != null)
            {
                QuitButton.onClick.AddListener(QuitGame);
            }
            else
            {
                Debug.Log("해당 오브젝트는 해당 Scene에 없습니다!");
            }

        }
    }
}
