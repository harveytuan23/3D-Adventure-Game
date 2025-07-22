using UnityEngine;

public class MouseCameraController : MonoBehaviour
{
    public Transform target; // 主角
    public Vector3 offset = new Vector3(0f, 3f, -6f);
    public float rotationSpeed = 5f;
    public float smoothSpeed = 10f;

    private float currentYaw = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // 根據滑鼠水平移動值改變 Y 軸角度（左右視角）
        currentYaw += Input.GetAxis("Mouse X") * rotationSpeed;

        // 計算攝影機的新位置
        Quaternion rotation = Quaternion.Euler(0f, currentYaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        // 平滑移動攝影機
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 攝影機永遠看著主角
        transform.LookAt(target);
    }
}
