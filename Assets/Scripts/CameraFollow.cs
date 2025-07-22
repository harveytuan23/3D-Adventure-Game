using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // 要跟隨的對象（你的主角）
    public Vector3 offset;        // 攝影機與主角之間的偏移距離
    public float smoothSpeed = 5f; // 平滑程度

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
