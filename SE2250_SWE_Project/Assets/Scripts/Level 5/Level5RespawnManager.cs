using UnityEngine;

public class Level5RespawnManager : MonoBehaviour
{
    [Tooltip("Set this to a Transform in your scene where the player should respawn if no checkpoint is reached yet.")]
    public Transform defaultCheckpoint;

    private Transform currentCheckpoint;

    public Transform CurrentCheckpoint
    {
        get
        {
            if (currentCheckpoint != null)
            {
                return currentCheckpoint;
            }

            if (defaultCheckpoint != null)
            {
                return defaultCheckpoint;
            }

            Debug.LogWarning("No checkpoint set and no defaultCheckpoint assigned.");
            return null;
        }
        set
        {
            currentCheckpoint = value;
        }
    }
}