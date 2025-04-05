using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Player Health")]
    public Health playerHealth;
    public Image playerHealthBar;

    [Header("Boss Health")]
    public BossHealth bossHealth;
    public Image bossHealthBar;

    void Update()
    {
        if (playerHealth != null && playerHealthBar != null)
        {
            float playerPercent = Mathf.Clamp01(playerHealth.GetHealthPercent());
            playerHealthBar.fillAmount = playerPercent;
        }

        if (bossHealth != null && bossHealthBar != null)
        {
            float bossPercent = Mathf.Clamp01(bossHealth.GetHealthPercent());
            bossHealthBar.fillAmount = bossPercent;
        }
    }
}