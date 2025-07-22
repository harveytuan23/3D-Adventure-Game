using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets;

public class NPCInteraction : MonoBehaviour
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

    [Header("鏡頭控制")]
    public Camera mainCamera;
    public Transform dialogueCamTransform;

    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private ThirdPersonCamera cameraControlScript;
    private ThirdPersonController playerControlScript;

    [Header("音效")]
    public AudioSource memorySound; // 拖入 AudioSource
    public AudioSource uiAudioSource;
    public AudioClip dialogueAdvanceClip;

    void Start()
    {
        cameraControlScript = Camera.main.GetComponent<ThirdPersonCamera>();
        playerControlScript = GameObject.FindWithTag("Player").GetComponent<ThirdPersonController>();
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

        if (playerControlScript != null)
            playerControlScript.enabled = false;

        if (cameraControlScript != null)
            cameraControlScript.enabled = false;
            
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        waitingForNext = false;
        
        if (!hasWelcomed)
        {
            dialogueStep = 100;
            ShowDialogueText("哈囉！歡迎來到蛋捲廣場！");
            hasWelcomed = true;
            waitingForNext = true;
        }
        else if (hasCompletedThisMemory)
        {
            ShowDialogueText("你已經找到這邊的記憶碎片了，繼續去其他地方尋找吧！");
            animator.SetTrigger("IdleToTalking");
            waitingForNext = true;
        }
        else if (basketTrigger != null && basketTrigger.peachCount >= 3)
        {
            dialogueStep = 1;
            ShowDialogueText("太棒了！你找到了全部重要的回憶！");
            animator.SetTrigger("FoundMemory");
            waitingForNext = true;
            hasCompletedThisMemory = true;
        }
        else
        {
            ShowDialogueText("你已經開始尋找了！還差一些，記得找出三個最重要的回憶喔～");
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
        if (dialogueStep == 1)
        {
            ShowDialogueText("這是屬於你跟段之間的記憶碎片…");
            animator.SetTrigger("IdleToTalking");
            dialogueStep = 2;
            waitingForNext = false;
            ShowMemoryFragment();
        }
        else if (dialogueStep == 100)
        {
            dialogueStep = 101;
            ShowDialogueText("如果想要獲得記憶碎片，就必須搜集三個最重要的回憶給我，並且放在回憶籃子裡面，準備好了就開始吧！");
            animator.SetTrigger("IdleToTalking");
            waitingForNext = true;
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

        InitialPlazaNPC.Instance.memoryCount++;
        InitialPlazaNPC.Instance.UpdateMemoryUI(); // ➕ 這行！


        isShowingMemoryPopup = true;

        StartCoroutine(FadeInMemoryPanelAfterDelay(2f, 1f)); // 2 秒延遲後，1 秒淡入
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

    void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;

        if (cameraControlScript != null)
            cameraControlScript.enabled = true;

        if (playerControlScript != null)
            playerControlScript.enabled = true;

        // ✅ 還原鏡頭位置與控制
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
