using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Data.Common;
using Sirenix.OdinInspector;

//제작자 : 방민호
public class MenuManager: SerializedMonoBehaviour
{
    [Header("관리 오브젝트 설정")]
    //리스트배열 형태로 이미지를 관리함
    [SerializeField] public List<Image> image = new List<Image>();
    // Start is called before the first frame update
    void Start()
    {
        //옵션 창 비활성화
        image[1].transform.gameObject.SetActive(false);
        image[2].transform.gameObject.SetActive(false);
    }
    //옵션 버튼을 눌렀을때 옵션창을 열게 만들 메서드를 지정
    public void OptionturnOn()
    {
        image[1].rectTransform.gameObject.SetActive(true);
    }
    //옵션닫기 버튼을 눌렀을때 옵션창을 닫게 만들 메서드 지정
    public void OptionturnOff()
    {
        image[1].rectTransform.gameObject.SetActive(false);
    }
    //게임실행을 시킬 메서드 지정
    public void GamestartButtonClick()
    {
        //로딩씬이 실행후에 게임씬으로 이동하도록 처리
        LoadingSceneManager.LoadScene("Game Scene");
    }
    public void MultiGameStartButtonClick()
    {
        image[2].transform.gameObject.SetActive(true);
    }
    //게임종료를 시킬 메서드 지정
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
        Application.OpenURL("");
    }
}
