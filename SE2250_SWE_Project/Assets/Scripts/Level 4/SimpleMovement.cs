using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float v = Input.GetAxis("Horizontal");
        float h = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(-h, 0, v);
        controller.Move(move * speed * Time.deltaTime);

        // ✅ Check if grounded FIRST, then apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to stay grounded
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}