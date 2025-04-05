using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 10.0f; // Example health
    private float currentHealth;

    private Collider myCollider; // Cache collider for notification

    void Awake()
    {
        currentHealth = maxHealth;
        myCollider = GetComponent<Collider>();
        if (myCollider == null)
        {
            Debug.LogError($"Enemy {name} needs a Collider component for interaction/notification!", this);
        }
    }

    // Example TakeDamage function (optional, not used by player interaction directly here)
    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return; // Already dead

        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Public method called by the PlayerInteraction script
    public void Die()
    {
        // Prevent multiple death calls
        if (currentHealth <= 0 && !gameObject.activeSelf) return; // Basic check if already 'dead' or inactive

        Debug.Log($"Bot {name} received Die() command.");
        currentHealth = 0; // Ensure health is 0

        // --- IMPORTANT: Notify Recruiter(s) BEFORE destroying ---
        NotifyRecruitersOnDeath();

        // --- Destroy the bot GameObject ---
        Debug.Log($"Destroying {name}.");
        Destroy(gameObject);

        // Alternatively, if you pool objects or just disable them:
        // gameObject.SetActive(false); // If you disable, the FixedUpdate in RecruiterDetection cleans up later
    }

    void NotifyRecruitersOnDeath()
    {
        if (myCollider == null)
        {
            Debug.LogWarning($"Cannot notify recruiters - {name} has no collider cached.");
            return;
        }

        // Find ALL RecruiterDetection scripts in the scene
        RecruiterDetection[] recruiters = FindObjectsOfType<RecruiterDetection>();
        if (recruiters.Length == 0)
        {
             // Debug.Log($"No RecruiterDetection scripts found in scene to notify."); // Optional log
             return;
        }


        Debug.Log($"Notifying {recruiters.Length} recruiter(s) about {name}'s death...");
        foreach (RecruiterDetection recruiter in recruiters)
        {
            // Tell each recruiter to remove this specific bot's collider from its list
            recruiter.ReportBotLeaving(myCollider);
        }
    }
}