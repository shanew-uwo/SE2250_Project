using UnityEngine;

public class WallAndFloor : MonoBehaviour
{
    public string wallTag = "Wall";    // Tag for walls
    public string floorTag = "Floor";  // Tag for floors
    public float wallFriction = 10f;  // Friction for wall-running or climbing
    public float floorFriction = 5f;  // Friction for walking on floors

    private Rigidbody playerRigidbody;

    void Start()
    {
        // Get the Rigidbody component attached to the player
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Check when player touches a surface
    private void OnCollisionStay(Collision collision)
    {
        // Check for wall collision
        if (collision.gameObject.CompareTag(wallTag))
        {
            // Apply wall friction to prevent the player from sliding through walls
            ApplyFriction(wallFriction);
        }

        // Check for floor collision
        if (collision.gameObject.CompareTag(floorTag))
        {
            // Apply floor friction to prevent the player from sliding through the floor
            ApplyFriction(floorFriction);
        }
    }

    // Apply friction to the player depending on the surface
    private void ApplyFriction(float frictionAmount)
    {
        // Limit the velocity in the X and Z axes (sideways and forward movement)
        Vector3 velocity = playerRigidbody.linearVelocity;
        velocity.x = Mathf.Lerp(velocity.x, 0, frictionAmount * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, 0, frictionAmount * Time.deltaTime);
        playerRigidbody.linearVelocity = velocity;
    }

    // Optionally, to detect when player leaves the surface and re-enable movement
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(wallTag) || collision.gameObject.CompareTag(floorTag))
        {
            // When the player leaves the surface, reset any applied friction
            ResetMovement();
        }
    }

    // Reset movement (velocity) after leaving the surface
    private void ResetMovement()
    {
        Vector3 velocity = playerRigidbody.linearVelocity;
        velocity.x = Mathf.Lerp(velocity.x, 0, 5f * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, 0, 5f * Time.deltaTime);
        playerRigidbody.linearVelocity = velocity;
    }
}
