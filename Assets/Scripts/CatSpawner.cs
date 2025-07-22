using UnityEngine;
using System.Collections.Generic;

public class CatSpawner : MonoBehaviour
{
    public GameObject catPrefab;
    public int numberOfCats = 10;
    public float spawnRadius = 10f;

    public Material normalMaterial;
    public Material bonnieMaterial;

    public bool assignBonnieHere = false; // ✅ 只在其中一個 spawner 勾選這個

    private static bool bonnieAssigned = false; // ✅ 全場唯一 Bonnie 控制 flag

    void Start()
    {
        List<GameObject> spawnedCats = new List<GameObject>();

        int spawned = 0;
        int maxAttempts = numberOfCats * 20;

        while (spawned < numberOfCats && maxAttempts-- > 0)
        {
            Vector3 pos = GetRandomPoint(transform.position, spawnRadius);

            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out UnityEngine.AI.NavMeshHit hit, 2.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                GameObject cat = Instantiate(catPrefab, hit.position, Quaternion.identity);
                spawnedCats.Add(cat);
                spawned++;
            }
        }

        // ✅ 如果這個 spawner 負責指定 Bonnie，且全場還沒指定過
        if (assignBonnieHere && !bonnieAssigned && spawnedCats.Count > 0)
        {
            int bonnieIndex = Random.Range(0, spawnedCats.Count);

            for (int i = 0; i < spawnedCats.Count; i++)
            {
                var cat = spawnedCats[i];
                var controller = cat.GetComponent<CatAIController>();
                var renderer = cat.GetComponentInChildren<SkinnedMeshRenderer>();

                if (i == bonnieIndex)
                {
                    controller.isBonnie = true;
                    cat.tag = "Bonnie";
                    if (renderer && bonnieMaterial)
                        renderer.material = bonnieMaterial;

                    bonnieAssigned = true; // ✅ 標記全場已經有 Bonnie
                    Debug.Log("🎯 Bonnie 已指定在: " + cat.name + "（來自 " + name + "）");
                }
                else
                {
                    controller.isBonnie = false;
                    if (renderer && normalMaterial)
                        renderer.material = normalMaterial;
                }
            }
        }
        else
        {
            // 其它 spawner 只生成 normal cat
            foreach (var cat in spawnedCats)
            {
                var controller = cat.GetComponent<CatAIController>();
                controller.isBonnie = false;

                var renderer = cat.GetComponentInChildren<SkinnedMeshRenderer>();
                if (renderer && normalMaterial)
                    renderer.material = normalMaterial;
            }
        }
    }

    Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector2 circle = Random.insideUnitCircle * radius;
        return new Vector3(center.x + circle.x, center.y + 1f, center.z + circle.y);
    }
}
