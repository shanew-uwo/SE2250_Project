using UnityEngine;

public class NPC_Animations_Script : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rb;
    public float walkThreshold = 0.01f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null || rb == null)
        {
            Debug.LogError("Animator or Rigidbody is missing!");
            return;
        }
        
        UpdateAnimationParameters();
    }
    
    public virtual void UpdateAnimationParameters()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        animator.SetFloat("Speed", speed);
        
    }
}
