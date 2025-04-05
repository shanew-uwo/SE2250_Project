using UnityEngine;

public class Level5Checkpoint : MonoBehaviour
{
    public string playerTag = "Player"; // Tag on the player GameObject

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Level5RespawnManager respawnManager = other.GetComponent<Level5RespawnManager>();
            if (respawnManager != null)
            {
                respawnManager.CurrentCheckpoint = this.transform;
                Debug.Log("Level5 Checkpoint updated to: " + this.name);
            }
        }
    }
}
