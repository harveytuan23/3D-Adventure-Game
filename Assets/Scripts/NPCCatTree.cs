using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets;

public class CatPlatformNPC : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject talkPromptUI;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject memoryPopupPanel;
    public TextMeshProUGUI memoryTipText;
    // public GameObject memoryCounterUI;
    // public TextMeshProUGUI memoryCounterText;
    public CanvasGroup locationTextGroup; // 拖入 LocationText
    public bool bonnieInBox = false; // 外部由 BoxTrigger 呼叫設為 true
    public GameObject memoryFragmentEffect; // 拖記憶碎片 UI

    [Header("動畫與邏輯")]
    public Animator animator;
    public BasketTrigger basketTrigger;

    [Header("狀態追蹤")]
    private bool playerInRange = false;
    private bool hasStoodUp = false;
    private bool hasTalked = false;
    private bool hasWelcomed = false;
    private bool isDialogueActive = false;
    private bool waitingForNext = false;
    private bool isShowingMemoryPopup = false;
    private bool hasCompletedThisMemory = false;
    // private int memoryCount = 0;
    private int dialogueStep = 0;

    [Header("音效")]
    public AudioSource memorySound; // 拖入 AudioSource
    public AudioSource uiAudioSource;
    public AudioClip dialogueAdvanceClip;

    public Camera mainCamera; // 拖入 Main Camera
    public Transform dialogueCamTransform; // 一個空物件，設定為鏡頭對話時的位置角度
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private ThirdPersonCamera cameraControlScript;
    private ThirdPersonController playerControlScript;

    void Start()
    {
        cameraControlScript = Camera.main.GetComponent<ThirdPersonCamera>();
        playerControlScript = GameObject.FindWithTag("Player").GetComponent<ThirdPersonController>();
    }

    public void ShowSimpleDialogue(string line)
    {
        StopAllCoroutines();
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeText(dialogueText, line));
    }


    void Update()
    {
        // 1️⃣ 優先處理記憶碎片彈窗
        if (isShowingMemoryPopup && Input.GetKeyDown(KeyCode.E))
        {
            memoryPopupPanel.SetActive(false);
            isShowingMemoryPopup = false;

            dialoguePanel.SetActive(true);
            ShowDialogueText("繼續搜集吧！你一定可以的！");
            animator.SetTrigger("IdleToTalking");
            isDialogueActive = true;
            waitingForNext = true;
            return;
        }

        // 2️⃣ 玩家在對話中 → 按 E 切換下一句
        if (isDialogueActive && waitingForNext && Input.GetKeyDown(KeyCode.E))
        {
            if (uiAudioSource != null && dialogueAdvanceClip != null)
            {
                uiAudioSource.PlayOneShot(dialogueAdvanceClip);
            }
            ContinueDialogue();
            return;
        }

        // 3️⃣ 玩家靠近並按下 E 啟動對話
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (uiAudioSource != null && dialogueAdvanceClip != null)
            {
                uiAudioSource.PlayOneShot(dialogueAdvanceClip);
            }

            if (!hasStoodUp)
            {
                animator.SetTrigger("StandUp");
                talkPromptUI.SetActive(false);
                hasStoodUp = true;
                StartCoroutine(ShowDialogueAfterAnimation(4.5f));
            }
            else if (hasTalked)
            {
                ShowDialogue();
            }
        }
    }

    void ShowDialogue()
    {
        // ✅ 鎖定鏡頭
        if (mainCamera != null && dialogueCamTransform != null)
        {
            originalCamPos = mainCamera.transform.position;
            originalCamRot = mainCamera.transform.rotation;

            mainCamera.transform.position = dialogueCamTransform.position;
            mainCamera.transform.rotation = dialogueCamTransform.rotation;
        }
        
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        waitingForNext = false;

        if (playerControlScript != null)
            playerControlScript.enabled = false;

        if (cameraControlScript != null)
            cameraControlScript.enabled = false;

        if (!hasWelcomed)
        {
            dialogueStep = 100;
            ShowDialogueText("歡迎來到巨大貓跳台，還記得你們是如何與Bonnie相遇的嗎？");
            hasWelcomed = true;
            waitingForNext = true;
        }
        else if (hasCompletedThisMemory)
        {
            ShowDialogueText("你已經找回這段記憶了，繼續探索其他回憶吧！");
            animator.SetTrigger("IdleToTalking");
            waitingForNext = true;
        }
        else if (bonnieInBox)
        {
            dialogueStep = 200;
            ShowDialogueText("恭喜你找到Bonnie了！在這麼多貓中選擇牠，就像當初的你一樣～");
            animator.SetTrigger("FoundMemory");
            waitingForNext = true;
        }
        else
        {
            ShowDialogueText("請在這巨大貓跳台中找到Bonnie！");
            animator.SetTrigger("IdleToTalking");
            waitingForNext = true;
        }
    }


    void ShowDialogueText(string text)
    {
        StopAllCoroutines(); // 避免重複啟動打字機
        StartCoroutine(TypeText(dialogueText, text));
    }

    void ContinueDialogue()
    {
        if (dialogueStep == 100)
        {
            dialogueStep = 101;
            ShowDialogueText("想要獲得記憶碎片，就必須找到Bonnie，準備好了就開始吧！");
            animator.SetTrigger("IdleToTalking");
            waitingForNext = true;
        }
        else if (dialogueStep == 200)
        {
            dialogueStep = 201;
            ShowDialogueText("來吧，這是屬於你與Bonnie的記憶碎片，記得收好喔！");
            animator.SetTrigger("IdleToTalking");
            waitingForNext = false;

            ShowMemoryFragment();
            hasCompletedThisMemory = true;
        }
        else if (dialogueStep == 201)
        {
            ShowDialogueText("想要獲得完整的回憶，就必須搜集到全部的記憶碎片喔，繼續加油吧！");
            waitingForNext = true;
            dialogueStep = 999;
        }
        else
        {
            CloseDialogue();
        }
    }

    void ShowMemoryFragment()
    {
        memoryPopupPanel.SetActive(true);
        memoryPopupPanel.GetComponent<CanvasGroup>().alpha = 0f;
        memoryTipText.text = "<size=20>記憶碎片</size>\n<size=12>搜集完全部的記憶碎片可以找回失去的回憶</size>";

        InitialPlazaNPC.Instance.memoryCount++; // 加這行 ✅
        InitialPlazaNPC.Instance.UpdateMemoryUI();

        isShowingMemoryPopup = true;

        StartCoroutine(FadeInMemoryPanelAfterDelay(2f, 1f)); // 2 秒延遲後，1 秒淡入
    }

    public void TriggerAutoCongrats()
    {
        if (hasCompletedThisMemory) return;

        bonnieInBox = true;
        hasTalked = true; // ✅ 讓玩家再按 E 時能直接跳對話
    }

    IEnumerator FadeInMemoryPanelAfterDelay(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        if (memorySound != null && !memorySound.isPlaying)
        {
            memorySound.Play();
        }

        CanvasGroup canvasGroup = memoryPopupPanel.GetComponent<CanvasGroup>();
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;

        if (playerControlScript != null)
            playerControlScript.enabled = true;

        if (cameraControlScript != null)
            cameraControlScript.enabled = true;

        // ✅ 還原鏡頭位置與角度
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCamPos;
            mainCamera.transform.rotation = originalCamRot;
        }
    }

    IEnumerator ShowDialogueAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowDialogue();
        hasTalked = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (!hasStoodUp)
            {
                talkPromptUI.SetActive(true);
            }
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
                // 遇到標籤，不打字、直接拼出來
                string tag = "";
                while (i < fullText.Length && fullText[i] != '>')
                {
                    tag += fullText[i];
                    i++;
                }
                tag += '>'; // 加上關閉符號
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

    public void ShowLocationTitle(string text, float fadeInTime = 1f, float holdTime = 2f, float fadeOutTime = 1f)
    {
        StartCoroutine(FadeLocationText(text, fadeInTime, holdTime, fadeOutTime));
    }

    IEnumerator FadeLocationText(string text, float fadeInTime, float holdTime, float fadeOutTime)
    {
        TextMeshProUGUI textComponent = locationTextGroup.GetComponent<TextMeshProUGUI>();
        textComponent.text = text;

        // 淡入
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            locationTextGroup.alpha = Mathf.Lerp(0, 1, t / fadeInTime);
            yield return null;
        }

        locationTextGroup.alpha = 1f;
        yield return new WaitForSeconds(holdTime);

        // 淡出
        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            locationTextGroup.alpha = Mathf.Lerp(1, 0, t / fadeOutTime);
            yield return null;
        }

        locationTextGroup.alpha = 0f;
    }


}
