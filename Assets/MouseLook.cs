using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public float MouseSensitivity = 100f;

   public Transform playerBody;

   float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Vector2 delta =  Mouse.current.delta.ReadValue(); 
        float mouseX = delta.x * (MouseSensitivity * Time.deltaTime);
        float mouseY = delta.y * (MouseSensitivity * Time.deltaTime);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = UnityEngine.Quaternion.Euler(xRotation , 0f, 0f);
        playerBody.Rotate(UnityEngine.Vector3.up * mouseX);

    }
}
