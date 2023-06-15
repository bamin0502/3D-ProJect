using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionDropdown : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    private int[] resolutionsWidths = { 2560, 1920, 1280 };
    private int[] resolutionsHeights = { 1440, 1080, 720 };
    private void Awake()
    {
        resolutionDropdown.value = 2;
    }
    private void Start()
    {
        // Toggle 요소의 이벤트에 메서드 연결
        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggleValueChanged(); });
        // Dropdown 요소에 3가지 해상도를 추가합니다.
        resolutionDropdown.ClearOptions();

        for (int i = 0; i < resolutionsWidths.Length; i++)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolutionsWidths[i].ToString() + "x" + resolutionsHeights[i].ToString()));
        }      
    }

    private void Update()
    {
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionDropdownValueChanged(); }); // Dropdown 이벤트 추가

    }
    private void OnResolutionDropdownValueChanged()
    {
        int resolutionIndex = resolutionDropdown.value;
        Screen.SetResolution(resolutionsWidths[resolutionIndex], resolutionsHeights[resolutionIndex], Screen.fullScreen);
    }
    private void OnFullscreenToggleValueChanged()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }
}