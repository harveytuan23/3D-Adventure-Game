using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class EndingSlide
{
    public Sprite image;
    [TextArea(2, 4)]
    public string[] texts;
}

public class EndingStoryManager : MonoBehaviour
{
    public Image storyImage;
    public TextMeshProUGUI storyText;
    public StorySlide[] slides;
    public float typingSpeed = 0.05f;
    public Image fadePanel;
    public float fadeDuration = 1f;
    public AudioSource audioA;
    public AudioClip bgm1;
    public float musicFadeDuration = 1f;
    public GameObject theEndText;



    private int slideIndex = 0;
    private int textIndex = 0;
    private bool isTyping = false;
    private bool textFullyDisplayed = false;

     void Start()
    {
        if (SceneManager.GetActiveScene().name != "Scene_Ending")
        {
            Debug.LogWarning("❌ Scene 還沒正確切換到 Ending，不啟動");
            return;
        }

        fadePanel.color = new Color(0, 0, 0, 1);
        fadePanel.gameObject.SetActive(true);
        StartCoroutine(PlaySlide());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                storyText.text = slides[slideIndex].texts[textIndex];
                isTyping = false;
                textFullyDisplayed = true;
            }
            else if (textFullyDisplayed)
            {
                ShowNextTextOrSlide();
            }
        }
    }

    void ShowNextTextOrSlide()
    {
        textIndex++;

        if (textIndex < slides[slideIndex].texts.Length)
        {
            storyText.text = "";
            StartCoroutine(TypeText(slides[slideIndex].texts[textIndex]));
        }
        else
        {
            slideIndex++;
            textIndex = 0;
            storyText.text = "";
            StartCoroutine(PlaySlide()); // ✅ 換下一張圖片的播放流程
        }
    }

    IEnumerator PlaySlide()
    {
        if (slideIndex >= slides.Length)
        {
            yield return StartCoroutine(FadeOut());

            if (theEndText != null)
                theEndText.SetActive(true);

            yield return new WaitForSeconds(10f); // 停留 10 秒

            yield return StartCoroutine(FadeOutMusic(audioA)); 
            Application.Quit(); // ✅ 結束遊戲
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 若在編輯器中，也停止播放
        #endif
            yield break;
        }

        if (slideIndex != 0)
            yield return StartCoroutine(FadeOut());

        storyImage.sprite = slides[slideIndex].image;
        storyImage.rectTransform.localScale = Vector3.one;
        storyText.text = "";

        yield return StartCoroutine(FadeIn());

        // 🎵 音樂播放邏輯
        if (slideIndex == 0)
        {
            StartCoroutine(FadeInMusic(audioA, bgm1)); // Slide 1 開始播 bgm1
        }
        // else if (slideIndex == 3)
        // {
        //     StartCoroutine(FadeOutMusic(audioA));      // Slide 3 結束 → A 淡出
        //     StartCoroutine(FadeInMusic(audioB, bgm2)); // Slide 4 開始播 bgm2
        // }

        StartCoroutine(ZoomInImage());
        StartCoroutine(TypeText(slides[slideIndex].texts[textIndex]));
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        textFullyDisplayed = false;
        storyText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            storyText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        textFullyDisplayed = true;
    }

    IEnumerator ZoomInImage()
    {
        float duration = 4f; // 拉近時間（秒）
        float time = 0f;

        storyImage.rectTransform.localScale = Vector3.one; // 初始 1 倍

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float zoomScale = Mathf.Lerp(1f, 1.1f, t); // 拉到 110%
            storyImage.rectTransform.localScale = new Vector3(zoomScale, zoomScale, 1f);
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        fadePanel.gameObject.SetActive(true);
        Color color = fadePanel.color;
        color.a = 1f;
        fadePanel.color = color;

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, time / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 0);
        fadePanel.gameObject.SetActive(false);
    }

    IEnumerator FadeOut()
    {
        fadePanel.gameObject.SetActive(true);
        Color color = fadePanel.color;
        color.a = 0f;
        fadePanel.color = color;

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 1);
    }

    IEnumerator FadeOutMusic(AudioSource source)
    {
        float startVolume = source.volume;

        float time = 0f;
        while (time < musicFadeDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / musicFadeDuration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    IEnumerator FadeInMusic(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.volume = 0f;
        source.Play();

        float time = 0f;
        while (time < musicFadeDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, 0.5f, time / musicFadeDuration);
            yield return null;
        }
    }

}
