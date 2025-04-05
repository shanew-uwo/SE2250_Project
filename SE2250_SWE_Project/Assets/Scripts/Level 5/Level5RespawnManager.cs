using UnityEngine;

public class Level5RespawnManager : MonoBehaviour
{
    public Transform defaultCheckpoint;  // Optional: fallback if no checkpoint reached

    private Transform currentCheckpoint;

    public Transform CurrentCheckpoint
    {
        get
        {
            return currentCheckpoint != null ? currentCheckpoint : defaultCheckpoint;
        }
        set
        {
            currentCheckpoint = value;
        }
    }
}