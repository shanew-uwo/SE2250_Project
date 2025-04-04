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
            originalColor = new Color(1f, 0f, 0f, 0.4f); // red with 40% transparency
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

        Debug.Log(damageOverlay != null ? "Damage overlay: " + damageOverlay.gameObject.name : "Damage overlay null");
        if (damageOverlay != null)
        {
            flashTimer = flashDuration;
            damageOverlay.color = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a);
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