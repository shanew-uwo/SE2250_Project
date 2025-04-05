using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Keep UI for player health flash
using System.Linq; // Needed for FindObjectsOfType

// Base class for anything that can take damage and die.
public class Health : MonoBehaviour
{
    [Header("Base Health Settings")]
    [SerializeField] protected float maxHealth = 100f; // Changed to protected
    protected float currentHealth; // Changed to protected

    [Header("Player Damage Visuals (Optional)")]
    [Tooltip("Assign the Player's damage overlay Image here.")]
    public Image damageOverlay; // Keep public for injector
    public float flashDuration = 0.2f;
    private float flashTimer = 0f;
    private Color originalOverlayColor;

    // --- Recruiter Notification ---
    protected Collider myCollider; // Cache collider for notification
    [Tooltip("Tag used by RecruiterDetection to identify relevant entities.")]
    [SerializeField] protected string recruiterBotTag = "Bot"; // Tag for recruiter notification

    public virtual void Start() // Make Start virtual
    {
        currentHealth = maxHealth;
        myCollider = GetComponent<Collider>(); // Cache collider

        if (CompareTag("Player")) // Only setup overlay for player
        {
            SetupDamageOverlay();
        }

        if (myCollider == null && CompareTag(recruiterBotTag))
        {
            Debug.LogWarning($"'{name}' is tagged '{recruiterBotTag}' for recruiter but missing a Collider component!", this);
        }
    }

    protected virtual void Update() // Make Update protected virtual
    {
        if (damageOverlay != null && flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            float alpha = Mathf.Lerp(0f, originalOverlayColor.a, flashTimer / flashDuration);
            damageOverlay.color = new Color(originalOverlayColor.r, originalOverlayColor.g, originalOverlayColor.b, alpha);
        }
    }

    public virtual void TakeDamage(float amount) // Make TakeDamage virtual
    {
        if (currentHealth <= 0) return; // Already dead

        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. Health: {currentHealth}/{maxHealth}");

        // Trigger flash only if it's the player and overlay exists
        if (CompareTag("Player") && damageOverlay != null)
        {
            flashTimer = flashDuration;
            damageOverlay.color = new Color(originalOverlayColor.r, originalOverlayColor.g, originalOverlayColor.b, originalOverlayColor.a);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // CRITICAL: Modified Die() method
    public virtual void Die()
    {
        // Prevent multiple death calls / redundant actions
        if (currentHealth > 0) currentHealth = 0; // Ensure health is 0 if called externally

        // --- 1. Notify Recruiters (IF this object is tagged as a 'Bot') ---
        // We only notify if this object IS a bot the recruiter cares about.
        if (myCollider != null && CompareTag(recruiterBotTag))
        {
            NotifyRecruitersOnDeath();
        }
        else if (CompareTag(recruiterBotTag)) // Log if tagged but no collider
        {
             Debug.LogWarning($"'{name}' tagged '{recruiterBotTag}' died but couldn't notify recruiters (missing Collider).");
        }

        // --- 2. Perform Death Action ---
        PerformDeathAction();
    }

    // Separated action for clarity and overriding
    protected virtual void PerformDeathAction()
    {
        // Default action: Reload scene (primarily for Player)
        Debug.Log($"{name} died. Reloading scene.");
        // Check if the object still exists before trying to destroy/reload
        // (It might have been destroyed by an external system after notification)
        if (this != null && gameObject != null)
        {
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    // --- Helper Methods ---

    // Public method to allow external setting of health (e.g., on spawn)
    public void SetMaxHealth(float newMaxHealth, bool setCurrentToMax = true)
    {
        maxHealth = newMaxHealth;
        if (setCurrentToMax)
        {
            currentHealth = maxHealth;
        }
        // Optional: Clamp current health if max is reduced below current
        // currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;


    private void SetupDamageOverlay()
    {
        if (damageOverlay != null)
        {
            // Using a slightly transparent red
            originalOverlayColor = new Color(1f, 0f, 0f, 0.4f);
            damageOverlay.color = new Color(originalOverlayColor.r, originalOverlayColor.g, originalOverlayColor.b, 0f); // Start invisible
        }
        else
        {
            // Attempt to find it if not assigned (optional, less robust)
            // GameObject canvasObj = GameObject.Find("Canvas"); // Be careful with Find
            // if (canvasObj) damageOverlay = canvasObj.transform.Find("DamageOverlay")?.GetComponent<Image>();
            // if (damageOverlay != null) SetupDamageOverlay(); // Recurse to setup color
            // else Debug.LogWarning("Player Health: DamageOverlay Image not assigned and couldn't be found automatically.", this);
        }
    }

    // --- Recruiter Notification Logic (from original EnemyHealth) ---
    protected void NotifyRecruitersOnDeath()
    {
        // Find ALL RecruiterDetection scripts in the scene
        // Using FindObjectsOfType is okay here as death is infrequent
        RecruiterDetection[] recruiters = FindObjectsOfType<RecruiterDetection>();

        if (recruiters.Length > 0)
        {
             Debug.Log($"'{name}' (Tag: {tag}) notifying {recruiters.Length} recruiter(s) about death...");
             foreach (RecruiterDetection recruiter in recruiters)
             {
                 // Tell each recruiter to remove this specific bot's collider
                 recruiter.ReportBotLeaving(myCollider);
             }
        }
         // Optional: Log if no recruiters found
         // else { Debug.Log($"'{name}' died, no RecruiterDetection scripts found in scene to notify."); }
    }
}