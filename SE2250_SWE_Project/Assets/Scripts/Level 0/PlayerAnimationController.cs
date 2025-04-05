using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerMovementController))]
public class CharacterAnimationController : MonoBehaviour
{
    public Animator animator;
    private Rigidbody rb;
    private PlayerMovementController playerMovement;

    [Header("Animation Thresholds")]
    public float fallThreshold = -0.1f;
    public float jumpThreshold = 0.1f;

    private void Start()
    {
        animator = animator ?? GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        if (animator == null || rb == null || playerMovement == null)
            return;

        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

        if (playerMovement.isGrounded)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }
        else
        {
            if (rb.linearVelocity.y > jumpThreshold)
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
}