using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Character Attributes")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f; // Increased to match Unity physics scale

    private Vector3 moveDirection;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public Rigidbody rb;
    public bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

        // Press J to increase jump force by 1
        if (Input.GetKeyDown(KeyCode.J))
        {
            BoostJumpForce(1f);
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
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
            RespawnAtStart();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void BoostJumpForce(float amount)
    {
        jumpForce += amount;
        Debug.Log("Jump Force Increased by Event: " + jumpForce);
    }

    public void RespawnAtStart()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        Debug.Log("Player respawned at initial position.");
    }
}