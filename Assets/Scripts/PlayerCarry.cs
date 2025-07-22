using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    public Transform carryPoint; // 玩家身上某個空物件，作為貓的位置
    private GameObject carriedCat = null;

    public float interactRange = 2f;
    public LayerMask catLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (carriedCat == null)
            {
                TryPickUpCat();
            }
            else
            {
                PutDownCat();
            }
        }
    }

    void TryPickUpCat()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange);
        Debug.Log("👀 嘗試撿貓，目前抱的是：" + (carriedCat == null ? "無" : carriedCat.name));


        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Cat"))
            {
                Debug.Log("🎒 嘗試抱起貓咪：" + hit.name);

                carriedCat = hit.gameObject;

                // 關閉 AI
                var agent = carriedCat.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null) agent.enabled = false;

                // 重力＆碰撞處理
                carriedCat.GetComponent<Rigidbody>().isKinematic = true;
                carriedCat.GetComponent<Collider>().enabled = false;

                // 抱起
                carriedCat.transform.SetParent(carryPoint);
                carriedCat.transform.localPosition = Vector3.zero;
                carriedCat.transform.localRotation = Quaternion.identity;

                break;
            }
        }

        if (carriedCat == null)
        {
            Debug.Log("😿 沒有找到可以抱的貓，確認貓有 Tag=Cat 且距離小於 " + interactRange);
        }
    }


    void PutDownCat()
    {
        Debug.Log("📦 貓已放下：" + carriedCat.name);

        carriedCat.transform.SetParent(null);
        carriedCat.GetComponent<Rigidbody>().isKinematic = false;
        carriedCat.GetComponent<Collider>().enabled = true;

        var agent = carriedCat.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = true;

        // ✅ 將貓推離玩家一點點（避免重疊）
        Vector3 dropOffset = transform.forward * 1.2f; // 玩家正前方 1.2 公尺
        carriedCat.transform.position = transform.position + dropOffset;

        carriedCat = null;
    }

    public GameObject GetCarriedCat()
    {
        return carriedCat;
    }
}
