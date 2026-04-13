using UnityEngine;

public class SimpleDesktopLook : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 2.5f;
    public float smoothTime = 0.03f;

    float xRotation = 0f;
    float currentMouseX;
    float currentMouseY;
    float mouseXVelocity;
    float mouseYVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float targetMouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float targetMouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        currentMouseX = Mathf.SmoothDamp(currentMouseX, targetMouseX, ref mouseXVelocity, smoothTime);
        currentMouseY = Mathf.SmoothDamp(currentMouseY, targetMouseY, ref mouseYVelocity, smoothTime);

        xRotation -= currentMouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * currentMouseX);
    }
}