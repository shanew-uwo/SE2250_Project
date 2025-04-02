using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField] private float mouseSensitivity = 10f;

    // Use Vector3 for 3D movement direction
    private Vector3 moveDirection;
    private float rotationY;

    // Update is called once per frame
    void Update()
    {
        // Call both handlers every frame
        HandleMovement();
        HandleRotation(); // <-- ADD THIS LINE
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Use Vector3 and assign directly
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Translate in local space based on input
        // W/S moves along local Z (forward/backward)
        // A/D moves along local X (left/right)
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        rotationY += mouseX * mouseSensitivity; // Accumulate rotation

        // Apply the rotation around the Y axis
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}