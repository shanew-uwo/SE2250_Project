using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Character Attributes")]
    public float moveSpeed = 5f;  
    public float jumpForce = 0.5f;

    private float defaultMoveSpeed;
    private float defaultJumpForce;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Save default stats
        defaultMoveSpeed = 5f;
        defaultJumpForce = 0.5f;

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;

        // Save starting transform
        initialPosition = transform.position;
        initialRotation = transform.rotation;
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
            IncreaseJumpForce(1f);
        }

        // Increase Speed (Press K)
        if (Input.GetKeyDown(KeyCode.K))
        {
            IncreaseSpeed(1f);
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
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Jump
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Mountain"))
        {
            Respawn();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void IncreaseJumpForce(float amount)
    {
        jumpForce += amount;
        Debug.Log("Jump Force Increased: " + jumpForce);
    }

    public void IncreaseSpeed(float amount)
    {
        moveSpeed += amount;
        Debug.Log("Move Speed Increased: " + moveSpeed);
    }

    void Respawn()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        Debug.Log("Respawned after hitting Mountain!");
    }
}
