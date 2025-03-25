using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rb;
    public float walkThreshold = 0.01f;
    public float fallThreshold = -0.01f; // Lower this to detect falling sooner

    private bool isGrounded = true;
    private bool isFalling = false;

    void Start()
    {
        animator = animator ?? GetComponent<Animator>();
        rb = rb ?? GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (animator == null || rb == null)
        {
            Debug.LogError("Animator or Rigidbody is missing!");
            return;
        }

        // Handle Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Handle transitions between Jump, Fall, and Walk
        UpdateAnimationParameters();
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 7f, rb.linearVelocity.z); // Jump force
        isGrounded = false;
        isFalling = false;
        animator.SetBool("IsJumping", true);
        animator.SetBool("IsFalling", false);
    }

    private void UpdateAnimationParameters()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        animator.SetFloat("Speed", speed);

        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.1f) // Moving up (jumping)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
            }
            else if (rb.linearVelocity.y < fallThreshold) // Falling
            {
                isFalling = true;
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", true);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isFalling = false;
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;

            // Ensure that Falling animation triggers as soon as we leave the ground
            if (rb.linearVelocity.y < fallThreshold)
            {
                isFalling = true;
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", true);
            }
        }
    }
}
