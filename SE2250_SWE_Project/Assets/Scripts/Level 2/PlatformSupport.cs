using UnityEngine;

public class PlatformSupport : MonoBehaviour
{
    public string playerTag = "Player"; // Tag of the player character
    private void OnCollisionStay(Collision collision)
    {
        // Check if the player is colliding with the platform
        if (collision.gameObject.CompareTag(playerTag))
        {
            // Get the player's Rigidbody component
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // Apply a force upwards to prevent falling through the platform
                // Adjust this force as needed
                Vector3 upwardForce = new Vector3(0f, 5f, 0f);
                playerRigidbody.AddForce(upwardForce, ForceMode.VelocityChange);
            }
        }
    }
}

