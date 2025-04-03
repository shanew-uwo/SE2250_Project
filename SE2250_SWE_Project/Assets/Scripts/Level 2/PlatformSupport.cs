using UnityEngine;

public class PlatformSupport : MonoBehaviour
{
    public string playerTag = "Player"; // Tag of the player character
    private bool isPlayerOnPlatform = false; // Track if the player is on the platform

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player is colliding with the platform
        if (collision.gameObject.CompareTag(playerTag))
        {
            // Enable platform support when player lands on the platform
            isPlayerOnPlatform = true;
            // Optionally, ensure the player stays on top of the platform (aligning Y position)
            AlignPlayerToPlatform(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the player leaves the platform
        if (collision.gameObject.CompareTag(playerTag))
        {
            // Disable platform support when player leaves
            isPlayerOnPlatform = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Check if the player is still on the platform and we want to prevent falling through
        if (isPlayerOnPlatform && collision.gameObject.CompareTag(playerTag))
        {
            // Optionally, ensure the player is aligned with the platform (to avoid sliding off)
            AlignPlayerToPlatform(collision.gameObject);
        }
    }

    private void AlignPlayerToPlatform(GameObject player)
    {
        // Align the player's Y position with the platform (if needed)
        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            // Optional: you can also ensure the player's velocity in the Y-axis doesn't cause them to sink
            Vector3 playerPosition = player.transform.position;
            Vector3 platformPosition = transform.position;

            // Adjust the Y position so the player stays on top of the platform
            playerPosition.y = platformPosition.y + 1f; // Adjust this value based on the platform's height
            player.transform.position = playerPosition;

            // Optionally, prevent any downward velocity from being added to the player while on the platform
            if (playerRigidbody.linearVelocity.y < 0)
            {
                playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);
            }
        }
    }
}


