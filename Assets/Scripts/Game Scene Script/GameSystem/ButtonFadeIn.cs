using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFadeIn : MonoBehaviour
{
    public GameObject[] buttonObjects;
    public TextMeshProUGUI[] textObjects;
    public float fadeInDuration = 5.0f;
    public float delayBetweenButtons = 1f;

    private Image[] images;

    private void Start()
    {
        images = new Image[buttonObjects.Length];
        for (int i = 0; i < buttonObjects.Length; i++)
        {
            images[i] = buttonObjects[i].GetComponent<Image>();
        }

        // 버튼 이미지들을 초기 상태로 설정 (투명)
        foreach (Image image in images)
        {
            Color imageColor = image.color;
            imageColor.a = 0f;
            image.color = imageColor;
        }

        // 텍스트들을 초기 상태로 설정 (투명)
        foreach (TextMeshProUGUI text in textObjects)
        {
            Color textColor = text.color;
            textColor.a = 0f;
            text.color = textColor;
        }

        // 버튼들과 텍스트들을 순차적으로 나타나게 함
        for (int i = 0; i < images.Length; i++)
        {
            float delay = i * delayBetweenButtons;

            images[i].gameObject.SetActive(true);
            images[i].DOColor(new Color(1f, 1f, 1f, 1f), fadeInDuration)
                .SetDelay(delay);

            textObjects[i].gameObject.SetActive(true);
            textObjects[i].DOColor(new Color(1f, 1f, 1f, 1f), fadeInDuration)
                .SetDelay(delay);
        }
    }
}
