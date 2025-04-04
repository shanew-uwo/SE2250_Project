using UnityEngine;

public class GameResetter : MonoBehaviour
{
    public PlayerStats playerStats;

    void Start()
    {
        playerStats.ResetRuntimeValues();
    }
}