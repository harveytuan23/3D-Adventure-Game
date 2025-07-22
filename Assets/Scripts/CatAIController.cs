using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CatAIController : MonoBehaviour
{
    public bool isBonnie = false;

    [Header("喵叫聲")]
    public AudioClip[] normalMeows;
    public AudioClip bonnieMeow;
    private AudioSource audioSource;

    [Header("自動走動")]
    private NavMeshAgent agent;
    public float wanderRadius = 5f;
    public float waitTimeMin = 1.5f;
    public float waitTimeMax = 4f;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    [Header("互動邏輯")]
    private bool playerInRange = false;
    private bool hasInteracted = false;

    [Header("轉場設定")]
    public CanvasGroup fadeCanvas;       // 黑幕 UI
    public Transform teleportTarget;     // 傳送點 (NPC 前)
    public GameObject player;            // 玩家物件

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        SetNewDestination();
    }

    void Update()
    {
        if (!agent.enabled || !agent.isOnNavMesh) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                SetNewDestination();
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                StartWaiting();
            }
        }

        // 玩家互動
        if (playerInRange && !hasInteracted && Input.GetKeyDown(KeyCode.E))
        {
            if (isBonnie)
            {
                hasInteracted = true;
                Debug.Log("💖 Bonnie：你終於找到我了，我等你好久了...");

                GetComponent<AudioSource>()?.PlayOneShot(bonnieMeow);
                
                if (bonnieMeow) audioSource.PlayOneShot(bonnieMeow);

                // 🎯 呼叫 NPC 的對話匡來顯示這句話
                StartCoroutine(ShowBonnieAndFade());
            }
            else
            {
                Debug.Log("😺 這不是 Bonnie，只是一般貓～");

                if (normalMeows.Length > 0)
                {
                    int index = Random.Range(0, normalMeows.Length);
                    audioSource.PlayOneShot(normalMeows[index]);
                }
            }
        }
    }

    void SetNewDestination()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void StartWaiting()
    {
        isWaiting = true;
        waitTimer = Random.Range(waitTimeMin, waitTimeMax);
        agent.ResetPath();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            PlayMeow(); // 靠近也可以先叫一下
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void PlayMeow()
    {
        if (audioSource == null) return;

        if (isBonnie && bonnieMeow != null)
        {
            audioSource.PlayOneShot(bonnieMeow);
        }
        else if (normalMeows.Length > 0)
        {
            int index = Random.Range(0, normalMeows.Length);
            audioSource.PlayOneShot(normalMeows[index]);
        }
    }

    IEnumerator FadeAndTeleport()
    {
        // 畫面變黑
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = t;
            yield return null;
        }

        // 傳送玩家
        player.transform.position = teleportTarget.position;
        player.transform.rotation = teleportTarget.rotation;

        yield return new WaitForSeconds(0.5f);

        // 淡入
        t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            fadeCanvas.alpha = t;
            yield return null;
        }

        FindObjectOfType<CatPlatformNPC>()?.TriggerAutoCongrats();

    }

    IEnumerator WaitBeforeFade(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FadeAndTeleport());
    }

    IEnumerator ShowBonnieAndFade()
    {
        // 顯示台詞
        CatPlatformNPC npc = FindObjectOfType<CatPlatformNPC>();
        if (npc != null)
        {
            npc.ShowSimpleDialogue("你終於找到我了，我等你好久了，趕快帶我回家吧！");
        }

        yield return new WaitForSeconds(3f);

        // 關掉對話匡
        npc?.CloseDialogue();

        // 黑幕轉場 + 傳送
        StartCoroutine(FadeAndTeleport());
    }

}
