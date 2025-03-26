using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rb;
    public float walkThreshold = 0.01f;
    public float fallThreshold = -0.01f;

    private bool isGrounded = true;

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

        UpdateAnimationParameters();
    }

    private void UpdateAnimationParameters()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        animator.SetFloat("Speed", speed);

        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.1f)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
            }
            else if (rb.linearVelocity.y < fallThreshold)
            {
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
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}