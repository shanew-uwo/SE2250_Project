using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Image damageOverlay;
    public float flashDuration = 0.2f;
    private float flashTimer = 0f;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;
        if (damageOverlay != null)
        {
            originalColor = damageOverlay.color;
            damageOverlay.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Start invisible
        }
    }

    private void Update()
    {
        if (damageOverlay != null && flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            float alpha = Mathf.Lerp(0f, originalColor.a, flashTimer / flashDuration);
            damageOverlay.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Taking damage");
        currentHealth -= amount;

        if (damageOverlay != null)
        {
            flashTimer = flashDuration;
            damageOverlay.color = originalColor;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}