using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Player Health")]
    public Health playerHealth; // Assign Player's Health/PlayerHealth component
    public Image playerHealthBar; // Assign Player's health bar Image

    [Header("Boss Health")]
    public BossHealth bossHealth; // Assign Boss's BossHealth component
    public Image bossHealthBar; // Assign Boss's health bar Image

    void Update()
    {
        // --- Player Health Bar ---
        if (playerHealth != null && playerHealthBar != null) // Check BOTH are assigned
        {
            // GetHealthPercent now exists in the base Health class
            float playerPercent = playerHealth.GetHealthPercent(); // Clamp is now done inside GetHealthPercent
            playerHealthBar.fillAmount = playerPercent;
        }
        // Optional: Log warning if references are missing
        // else if (playerHealth == null) Debug.LogWarning("HealthBarUI: Player Health reference missing!");
        // else if (playerHealthBar == null) Debug.LogWarning("HealthBarUI: Player Health Bar Image missing!");


        // --- Boss Health Bar ---
        if (bossHealth != null && bossHealthBar != null) // Check BOTH are assigned
        {
            // BossHealth inherits GetHealthPercent from the base Health class
            float bossPercent = bossHealth.GetHealthPercent(); // Clamp is now done inside GetHealthPercent
            bossHealthBar.fillAmount = bossPercent;
        }
        // Optional: Log warning if references are missing
        // else if (bossHealth == null) Debug.LogWarning("HealthBarUI: Boss Health reference missing!");
        // else if (bossHealthBar == null) Debug.LogWarning("HealthBarUI: Boss Health Bar Image missing!");
    }
}