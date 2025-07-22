using UnityEngine;

public class BasketTrigger : MonoBehaviour
{
    public int peachCount = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("peach")) // or check by component
        {
            peachCount++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("peach"))
        {
            peachCount--;
        }
    }

}
