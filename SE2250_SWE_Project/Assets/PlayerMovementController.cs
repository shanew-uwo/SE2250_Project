using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Character Attributes")]
    public float moveSpeed = 5f;  // Default movement speed
    public float jumpForce = 0.5f; // Default jump force

    private float defaultMoveSpeed;
    private float defaultJumpForce;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ✅ Reset speed & jump force at game start
        defaultMoveSpeed = 5f;
        defaultJumpForce = 0.5f;

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
    }

    void Update()
    {
        ProcessInputs();
        RotateCharacter();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Increase Jump Force (Press J)
        if (Input.GetKeyDown(KeyCode.J))
        {
            IncreaseJumpForce(1f); // Increase jump force by 1
        }

        // Increase Speed (Press K)
        if (Input.GetKeyDown(KeyCode.K))
        {
            IncreaseSpeed(1f); // Increase speed by 1
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;
    }

    void ApplyMovement()
    {
        Vector3 moveVelocity = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    void RotateCharacter()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Reset vertical momentum
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // ✅ Increase jump force dynamically (J key)
    public void IncreaseJumpForce(float amount)
    {
        jumpForce += amount;
        Debug.Log("Jump Force Increased: " + jumpForce);
    }

    // ✅ Increase movement speed dynamically (K key)
    public void IncreaseSpeed(float amount)
    {
        moveSpeed += amount;
        Debug.Log("Move Speed Increased: " + moveSpeed);
    }
}
