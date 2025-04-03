using UnityEngine;

public class RespawnOnTouch : MonoBehaviour
{
    public Transform respawnPoint; // The respawn point where the character will go
    public string playerTag = "Player"; // Tag for identifying the character, ensure your player has this tag

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object colliding with the plane has the "Player" tag
        if (collision.gameObject.CompareTag(playerTag))
        {
            // Teleport the player to the respawn point
            collision.gameObject.transform.position = respawnPoint.position;
            collision.gameObject.transform.rotation = respawnPoint.rotation; // Optionally reset rotation
        }
    }
}
