using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public TextMeshPro timerText;
    private float elapsedTime = 0f;
    private bool isRunning = true;

    [Header("Player Stats")]
    public PlayerStats playerStats; // Drag your PlayerStatsAsset here

    [Header("Game State")]
    public GameState gameState; // Drag GameStateAsset here

    void Start()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = "Current Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("Timer stopped at: " + timerText.text);
        EvaluateTime();
    }

    void EvaluateTime()
    {
        float bonus = 0f;
        if (elapsedTime < 20f)
        {
            bonus = 1f;
            gameState.notificationMessage = "Jump Boost Upgrade: +1";
        }
        else if (elapsedTime >= 20f && elapsedTime <= 60f)
        {
            bonus = 0.75f;
            gameState.notificationMessage = "Jump Boost Upgrade: +0.75";
        }
        else
        {
            bonus = 0.5f;
            gameState.notificationMessage = "Jump Boost Upgrade: +0.5";
        }

        playerStats.IncreaseJumpForce(bonus);
        gameState.showNotification = true;
    }
}