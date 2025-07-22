using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using StarterAssets;
using UnityEngine.SceneManagement;

public class InitialPlazaNPC : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject talkPromptUI;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Image fadePanel;

    [Header("動畫")]
    public Animator animator;

    [Header("音效")]
    public AudioSource uiAudioSource;
    public AudioClip dialogueAdvanceClip;

    [Header("鏡頭控制")]
    public Camera mainCamera;
    public Transform dialogueCamTransform;
    private ThirdPersonCamera cameraControlScript;
    private ThirdPersonController playerControlScript;

    [Header("記憶碎片紀錄")]
    public int memoryCount = 0;

    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    private bool playerInRange = false;
    private bool hasTalked = false;
    private bool isDialogueActive = false;
    private bool waitingForNext = false;
    private int dialogueStep = 0;
    public float fadeDuration = 1f;

    // 結局專用對話流程
    private int endingDialogueStep = 0;
    private bool inEndingSequence = false;

    public static InitialPlazaNPC Instance;

    public TextMeshProUGUI memoryCounterText;

    public void UpdateMemoryUI()
    {
        if (memoryCounterText != null)
        {
            memoryCounterText.text = $"x {memoryCount}";
        }
    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 讓它切場景不消失
        }
        else
        {
            Destroy(gameObject); // 若已有，就刪掉重複的
        }
    }

    void Start()
    {
        cameraControlScript = Camera.main.GetComponent<ThirdPersonCamera>();
        playerControlScript = GameObject.FindWithTag("Player").GetComponent<ThirdPersonController>();

        if (fadePanel != null)
        {
            Color initColor = new Color(0f, 0f, 0f, 0f);
            fadePanel.color = initColor;
            fadePanel.gameObject.SetActive(false); // 避免提前顯示
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (uiAudioSource && dialogueAdvanceClip)
                uiAudioSource.PlayOneShot(dialogueAdvanceClip);

            if (!hasTalked)
            {
                ShowDialogue();
                hasTalked = true;
            }
            else if (isDialogueActive && waitingForNext)
            {
                if (inEndingSequence)
                    ShowNextEndingDialogue();
                else
                    ShowNextDialogue();
            }
            else if (!isDialogueActive)
            {
                ShowDialogue();
            }
        }
    }

    void ShowDialogue()
    {
        talkPromptUI.SetActive(false);
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        waitingForNext = false;

        if (playerControlScript != null)
            playerControlScript.enabled = false;

        if (cameraControlScript != null)
            cameraControlScript.enabled = false;

        if (mainCamera != null && dialogueCamTransform != null)
        {
            originalCamPos = mainCamera.transform.position;
            originalCamRot = mainCamera.transform.rotation;
            mainCamera.transform.position = dialogueCamTransform.position;
            mainCamera.transform.rotation = dialogueCamTransform.rotation;
        }

        if (hasTalked)
        {
            CheckFinalMemory();
            return;
        }

        dialogueStep = 0;
        ShowNextDialogue();
    }

    void ShowNextDialogue()
    {
        string line = "";

        switch (dialogueStep)
        {
            case 0:
                line = "歡迎來到回憶之丘";
                break;
            case 1:
                line = "你...應該還記得我是誰吧？";
                break;
            case 2:
                line = "....";
                break;
            case 3:
                line = "我是你的男朋友呀！居然連這都忘？";
                break;
            case 4:
                line = "<size=14>(算了算了，他常常忘東忘西)</size>";
                break;
            case 5:
                line = "如果你想要找回屬於我們之間的回憶，";
                break;
            case 6:
                line = "就必須要蒐集完全部的記憶碎片，並且把它們交給我。";
                break;
            case 7:
                line = "我就可以幫助你找回我們之間重要的回憶了。";
                break;
            case 8:
                line = "不用擔心，我會在記憶碎片出現的地方等你。";
                break;
            case 9:
                line = "現在就出發吧！";
                break;
            default:
                CloseDialogue();
                return;
        }

        if (animator != null && (dialogueStep == 0 || dialogueStep == 7))
            animator.SetTrigger("IdleToTalking");
        else if (animator != null && dialogueStep == 3)
            animator.SetTrigger("IdleToArguing");
        else if (animator != null && dialogueStep == 5)
            animator.SetTrigger("IdleToTalking2");
        else if (animator != null && dialogueStep == 9)
            animator.SetTrigger("IdleToExited");

        dialogueStep++;
        ShowDialogueText(line);
        waitingForNext = true;
    }

    void ShowNextEndingDialogue()
    {
        string line = "";

        switch (endingDialogueStep)
        {
            case 0:
                line = "恭喜你找到全部的記憶碎片！";
                break;
            case 1:
                line = "現在就讓我們一起把這些碎片拼在一起...";
                break;
            case 2:
                line = "看看會發生什麼事吧！";
                break;
            case 3:
                StartCoroutine(PlayEndingWithFadeOut());
                inEndingSequence = false;
                return;
            default:
                return;
        }

        endingDialogueStep++;
        ShowDialogueText(line);
        waitingForNext = true;
    }

    IEnumerator PlayEndingWithFadeOut()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeToBlack());
        Debug.Log("🎉 結局演出準備中！");
        // 這裡可加入切換場景或圖片回顧
    }

    void ShowDialogueText(string text)
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(dialogueText, text));
    }

    IEnumerator TypeText(TextMeshProUGUI targetText, string fullText, float typeSpeed = 0.03f)
    {
        targetText.text = "";
        bool insideTag = false;

        for (int i = 0; i < fullText.Length; i++)
        {
            char c = fullText[i];

            if (c == '<') insideTag = true;

            if (insideTag)
            {
                string tag = "";
                while (i < fullText.Length && fullText[i] != '>')
                {
                    tag += fullText[i];
                    i++;
                }
                tag += '>';
                targetText.text += tag;
                insideTag = false;
            }
            else
            {
                targetText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
        }
    }

    void CheckFinalMemory()
    {
        if (memoryCount < 2)
        {
            ShowDialogueText("你還沒有找到全部的記憶碎片喔！");
            if (animator) animator.SetTrigger("IdleToTalking");
            waitingForNext = true;
        }
        else
        {
            inEndingSequence = true;
            endingDialogueStep = 0;
            ShowNextEndingDialogue();
        }
    }

    IEnumerator FadeToBlack()
    {
        if (fadePanel == null)
        {
            Debug.LogWarning("⚠️ fadePanel 尚未設定！");
            yield break;
        }

        fadePanel.gameObject.SetActive(true);

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            fadePanel.color = new Color(0f, 0f, 0f, alpha); // 直接設定新的 color
            yield return null;
        }

        fadePanel.color = new Color(0f, 0f, 0f, 1f); // 保險地設定最後值

        SceneManager.LoadScene("Scene_Ending", LoadSceneMode.Single);
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        waitingForNext = false;

        if (playerControlScript != null)
            playerControlScript.enabled = true;

        if (cameraControlScript != null)
            cameraControlScript.enabled = true;

        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCamPos;
            mainCamera.transform.rotation = originalCamRot;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            talkPromptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            talkPromptUI.SetActive(false);
        }
    }
}
