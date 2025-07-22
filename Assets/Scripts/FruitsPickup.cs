using UnityEngine;

public class FruitPickupSystem : MonoBehaviour
{
    public Transform holdPoint; // 拿水果的位置
    private GameObject heldFruit;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldFruit == null)
                TryPickup();
            else
                DropFruit();
        }
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Fruits"))
            {
                heldFruit = hit.gameObject;

                heldFruit.GetComponent<Rigidbody>().isKinematic = true;
                heldFruit.GetComponent<Collider>().enabled = false;

                heldFruit.transform.SetParent(holdPoint);
                heldFruit.transform.localPosition = Vector3.zero;
                heldFruit.transform.localRotation = Quaternion.identity;

                break;
            }
        }
    }

    void DropFruit()
    {
        heldFruit.transform.SetParent(null);

        Rigidbody rb = heldFruit.GetComponent<Rigidbody>();
        Collider col = heldFruit.GetComponent<Collider>();

        rb.isKinematic = false;
        col.enabled = true;

        // 拋出去一點點
        rb.AddForce(transform.forward * 2f, ForceMode.Impulse);

        heldFruit = null;
    }
}
