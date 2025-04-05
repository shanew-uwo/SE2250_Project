using UnityEngine;

public class Level5RespawnOnHazard : MonoBehaviour
{
    public string playerTag = "Player"; // Tag on the player GameObject

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            Level5RespawnManager respawnManager = collision.gameObject.GetComponent<Level5RespawnManager>();
            if (respawnManager != null && respawnManager.CurrentCheckpoint != null)
            {
                collision.gameObject.transform.position = respawnManager.CurrentCheckpoint.position;
                collision.gameObject.transform.rotation = respawnManager.CurrentCheckpoint.rotation;
            }
            else
            {
                Debug.LogWarning("No checkpoint set or Level5RespawnManager not found.");
            }
        }
    }
}