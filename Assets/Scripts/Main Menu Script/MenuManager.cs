using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Data.Common;

public class MenuManager: MonoBehaviour
{
   
    [Header("ȯ�� �ɼ� ����â")]
    //Ǯ��ũ�� ���� ���� ����
    //FullScreenMode screenMode;
    //Ǯ��ũ�� ���� ��� ����
    //public Toggle fullscreenBtn;
    //������ ���� ����
    //private int framerate;
    //�ػ� ���� ����Ʈ�迭 ����
    //List<Resolution> resolutions = new List<Resolution>();
    //��Ӵٿ� ���� ����
    //public TMP_Dropdown resolutionDropdown;
    //�ػ󵵸� �����Ҽ� �ְ� ���ִ� ���� ����
    int resolutionNum;
    //������� �����̴� ����
    [SerializeField] public Slider BackGroundMusic;
    //����Ʈ ���� �����̴� ����
    [SerializeField] public Slider SoundEffect;
    //����� ����
    [SerializeField] public AudioSource BackGround;
    //����Ʈ ���� ����
    [SerializeField] public AudioSource EffectSound;

    [Header("���� ������Ʈ ����")]
    //����Ʈ �迭 ���·� �����̴��� ������
    [SerializeField] public List<Slider> slider = new List<Slider>();
    //����Ʈ�迭 ���·� �̹����� ������
    [SerializeField] public List<Image> image = new List<Image>();

    // Start is called before the first frame update
    void Start()
    {
        //�ɼ� â ��Ȱ��ȭ
        image[1].transform.gameObject.SetActive(false);
        
    }
    // Update is called once per frame
    void Update()
    {
        //ChangeSize();
    }
    void Awake()
    {


    }
    //�ɼ� ��ư�� �������� �ɼ�â�� ���� ���� �޼��带 ����
    public void OptionturnOn()
    {
        image[1].transform.gameObject.SetActive(true);
    }
    //�ɼǴݱ� ��ư�� �������� �ɼ�â�� �ݰ� ���� �޼��� ����
    public void OptionturnOff()
    {
        image[1].rectTransform.gameObject.SetActive(false);
    }
    //���ӽ����� ��ų �޼��� ����
    public void GamestartButtonClick()
    {
        //�ε����� �����Ŀ� ���Ӿ����� �̵��ϵ��� ó��
        LoadingSceneManager.LoadScene("Game Scene");
    }
    //�������Ḧ ��ų �޼��� ����
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("���� ���� Ȯ�ο�");
    }
    //����� ������ �޼��� ����
    //public void ChangeSize()
    //{
    //    for (int i = 0; i < Screen.resolutions.Length; i++)
    //    {
    //        if (Screen.resolutions[i].refreshRate == 60 || Screen.resolutions[i].refreshRate == 165 || Screen.resolutions[i].refreshRate == 144)
    //            resolutions.Add(Screen.resolutions[i]);
    //    }
    //    resolutionDropdown.options.Clear();
    //    //�ʱ� ������ ����
    //    int optionNum = 0;
    //    foreach (Resolution item in resolutions)
    //    {
    //        TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
    //        option.text = item.width + "X" + item.height + " " + item.refreshRate + "hz" + "(BETA)";
    //        resolutionDropdown.options.Add(option);

    //        if (item.width == Screen.width && item.height == Screen.height)
    //            resolutionDropdown.value = optionNum;
    //        optionNum++;
    //    }
    //    resolutionDropdown.RefreshShownValue();
    //    //Ǯ��ũ�� ��ư�� Ŭ�����θ� ��ȯ��(���� ���¸� true �ƴϸ� false ��ȯ)
    //    fullscreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    //}
    //�ش� ��Ӵٿ� value ��ȣ�� �°� �����ϰ� ���ִ� �޼��� ����
    //public void DropboxOptionChange(int x)
    //{
    //    resolutionNum = x;
    //}
    ////Ȯ�ι�ư�� �������� Ȯ���ϴ� �޼��� ����
    //public void OKBtnClick()
    //{
    //    Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, screenMode);
    //}
    ////Ǯ��ũ������ �ƴ��� ���θ� �Ǵ��ϴ� �޼��� ����
    //public void FullScreenBtn(bool isFull)
    //{
        
    //    screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    //}
    //public void changeButtonClick()
    //{
    //    
    //}
}
