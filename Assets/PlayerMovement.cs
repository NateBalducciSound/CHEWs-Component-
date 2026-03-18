using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.8f;

    private Vector3 velocity;
    private bool isGrounded;

    void Awake()
    {
        // Auto-assign controller if missing
        if (!controller)
        {
            controller = GetComponent<CharacterController>();
            if (!controller)
                Debug.LogError("ERROR: No CharacterController found on Player!");
        }
    }

    void Update()
    {
        if (!controller) return;

        // ===== GROUNDED CHECK =====
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // ===== INPUT =====
        var kb = Keyboard.current;
        float moveX = (kb.aKey.isPressed ? -1 : 0) + (kb.dKey.isPressed ? 1 : 0);
        float moveY = (kb.sKey.isPressed ? -1 : 0) + (kb.wKey.isPressed ? 1 : 0);

        Vector3 move = transform.right * moveX + transform.forward * moveY;

        // ===== MOVE =====
        controller.Move(move * speed * Time.deltaTime);

        // ===== GRAVITY =====
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
