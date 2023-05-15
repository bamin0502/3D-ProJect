using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class ResolutionDropdown : SerializedMonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    private int[] resolutionsWidths = { 2560, 1920, 1280 };
    private int[] resolutionsHeights = { 1440, 1080, 720 };

    private void Start()
    {
        // Dropdown 요소에 3가지 해상도를 추가합니다.
        resolutionDropdown.ClearOptions();

        for (int i = 0; i < resolutionsWidths.Length; i++)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolutionsWidths[i].ToString() + "x" + resolutionsHeights[i].ToString()));
        }

        resolutionDropdown.value = 2; 
       
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