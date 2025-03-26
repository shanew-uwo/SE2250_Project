using UnityEngine;


public class NPC_Animations_Script : MonoBehaviour
{
    public Animator animator;
    public float speedMultiplier = 10f; // Adjust to control animation speed sensitivity

    private Vector3 lastPosition;
    private float speed;

    void Start()
    {
        lastPosition = transform.position; // Store initial position
    }

    void Update()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is missing!");
            return;
        }

        UpdateAnimationParameters();
    }

    public virtual void UpdateAnimationParameters()
    {
        // Calculate movement speed using distance covered per frame
        speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        // Apply multiplier if needed
        animator.SetFloat("Speed", speed * speedMultiplier);

        // Update last position for the next frame
        lastPosition = transform.position;
    }
}
