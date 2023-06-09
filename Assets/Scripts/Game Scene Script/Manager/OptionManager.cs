using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Data.Common;

public class OptionManager : MonoBehaviour
{
    [Header("환경 옵션 설정창")]
    //풀스크린 지정 변수 선언
    FullScreenMode screenMode;
    //풀스크린 전용 토글 선언
    public Toggle fullscreenBtn;
    //프레임 변수 지정
    private int framerate;
    //해상도 관련 리스트배열 선언
    List<Resolution> resolutions = new List<Resolution>();
    //드롭다운 변수 선언
    public TMP_Dropdown resolutionDropdown;
    //해상도를 변경할수 있게 해주는 변수 선언
    int resolutionNum;
    //인스턴스화 정적으로 선언
    //[SerializeField] public static GameManager instance = null;
    //배경음악 슬라이더 지정
    //[SerializeField] public Slider BackGroundMusic;
    //이펙트 사운드 슬라이더 지정
    //[SerializeField] public Slider SoundEffect;
    //배경음 지정
    //[SerializeField] public AudioSource BackGround;
    //이펙트 사운드 지정
    //[SerializeField] public AudioSource EffectSound;

    //[Header("관리 오브젝트 설정")]
    //리스트 배열 형태로 슬라이더를 관리함
    //[SerializeField] public List<Slider> slider = new List<Slider>();
    //리스트배열 형태로 이미지를 관리함
    //[SerializeField] public List<Image> image = new List<Image>();

    // Start is called before the first frame update
    void Start()
    {
        //옵션 창 비활성화
        //image[1].transform.gameObject.SetActive(false);
        ChangeSize();
    }
    void Awake()
    {
        //if (instance == null)//instance가 null. 즉 시스템상에 존재하고 있지 않은 상태라면
        //{
        //    instance = this;//내 자신을 instance에 넣어준다
        //    DontDestroyOnLoad(gameObject);//OnLoad(씬이 로드 되었을때) 자신을 파괴하지않고 유지한다.

        //}
        //else
        //{
        //    if(instance != this)//instance가 내가 아니라면 이미 instance가 존재하고 있다는 뜻이고
        //    {
        //        Destroy(this.gameObject);//둘이상 존재할시 안되기 때문에 방금전에 만들어진 본인을 파괴함
        //    }
        //}
    }
    //옵션 버튼을 눌렀을때 옵션창을 열게 만들 메서드를 지정
    //사이즈를 조절할 메서드 선언
    public void ChangeSize()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRate == 60 || Screen.resolutions[i].refreshRate == 165 || Screen.resolutions[i].refreshRate == 144)
                resolutions.Add(Screen.resolutions[i]);
        }
        resolutionDropdown.options.Clear();
        //초기 설정값 지정
        int optionNum = 0;
        foreach (Resolution item in resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = item.width + "X" + item.height + " " + item.refreshRate + "hz" + "(BETA)";
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionNum;
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();
        //풀스크린 버튼의 클릭여부를 반환함(눌린 상태면 true 아니면 false 반환)
        fullscreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }
    //해당 드롭다운 value 번호에 맞게 변경하게 해주는 메서드 생성
    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
    }
    //확인버튼을 누르는지 확인하는 메서드 생성
    public void OKBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, screenMode);
    }
    //풀스크린인지 아닌지 여부를 판단하는 메서드 생성
    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

}
