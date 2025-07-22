using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance;

    public int memoryCount = 0;

    void Awake()
    {
        // 建立單例 Singleton，確保遊戲中只有一個 MemoryManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 避免切場景時被刪除
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMemory()
    {
        memoryCount++;
        Debug.Log($"📦 已獲得記憶碎片！目前總數：{memoryCount}");
    }

    public int GetMemoryCount()
    {
        return memoryCount;
    }
}
