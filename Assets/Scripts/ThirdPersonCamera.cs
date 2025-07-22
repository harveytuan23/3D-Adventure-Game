using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;       // 主角
    public Vector3 offset = new Vector3(0f, 2f, -5f);
    public float rotationSpeed = 5f;

    private float yaw = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        yaw += Input.GetAxis("Mouse X") * rotationSpeed;

        Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f); // 看主角頭上

        // 給角色用來對齊移動方向（鏡頭 Transform）
        // 可把這個 yaw 轉給 PlayerMovement 使用
    }
}
