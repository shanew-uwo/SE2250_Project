using UnityEngine;

public class MovableBlock : Block, iMoveable
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    private void Start()
    {
        // Get the Rigidbody component attached to the block
        rb = GetComponent<Rigidbody>();
        
        // Initialize the Position with the object's starting position (optional)
        Position = transform.position;
    }

    // This method can be used to move the block based on external forces
    public void Move(Vector3 direction)
    {
        // Apply force to the Rigidbody in the given direction
        rb.AddForce(direction * moveSpeed, ForceMode.Impulse);
    }

    // Detect when another object collides with this block
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the block is being pushed by something else
        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            // Get the direction of the collision
            Vector3 pushDirection = collision.contacts[0].normal; 
            // Reverse the direction to push the block away from the object
            Vector3 pushForce = -pushDirection;
            
            // Apply the movement (push the block)
            Move(pushForce);
        }
    }
}