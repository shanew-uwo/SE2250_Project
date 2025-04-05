using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Image
using System.Linq;    // Required for FindObjectsOfType

/// <summary>
/// Base class for any entity that can take damage, die, and potentially notify others.
/// Handles health management, damage taking, death sequence (including notifications),
/// and optional player visual feedback.
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Base Health Settings")]
    [SerializeField] protected float maxHealth = 100f;
    // currentHealth is initialized in Start or by SetMaxHealth
    protected float currentHealth;

    [Header("Player Damage Visuals (Optional)")]
    [Tooltip("Assign the Player's damage overlay Image here (e.g., via PlayerBehaviorInjector).")]
    public Image damageOverlay; // Public for external assignment
    [SerializeField] private float flashDuration = 0.2f;
    private float flashTimer = 0f;
    private Color originalOverlayColor;

    [Header("Recruiter Integration (Optional)")]
    [Tooltip("Tag used by RecruiterDetection scripts to identify entities they track (e.g., 'Bot').")]
    [SerializeField] protected string recruiterBotTag = "Bot";
    protected Collider myCollider; // Cached collider for notifications & interactions

    // --- Initialization ---

    public virtual void Start()
    {
        // Initialize current health *if* it hasn't already been set
        // by a derived class calling SetMaxHealth before base.Start().
        // A simple check: if currentHealth is still 0 (default) or less, set it to max.
        // Note: Derived classes should ideally call SetMaxHealth *before* base.Start().
        if (currentHealth <= 0 && maxHealth > 0)
        {
            currentHealth = maxHealth;
        }

        myCollider = GetComponent<Collider>();

        // Setup visuals only if this is the player
        if (CompareTag("Player"))
        {
            SetupDamageOverlay();
        }

        // Warning if tagged for recruiter but missing collider needed for notification
        if (myCollider == null && CompareTag(recruiterBotTag))
        {
            Debug.LogWarning($"'{name}' is tagged '{recruiterBotTag}' but missing a Collider component, cannot notify recruiters on death!", this);
        }
    }

    // --- Frame Update ---

    protected virtual void Update()
    {
        // Handle the player damage overlay flash fadeout
        if (damageOverlay != null && flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            float alpha = Mathf.Lerp(0f, originalOverlayColor.a, flashTimer / flashDuration);
            damageOverlay.color = new Color(originalOverlayColor.r, originalOverlayColor.g, originalOverlayColor.b, alpha);
        }
    }

    // --- Damage & Death ---

    /// <summary>
    /// Applies damage to this entity. Triggers death sequence if health drops to 0 or below.
    /// Can be overridden by derived classes for custom damage reactions.
    /// </summary>
    /// <param name="amount">The amount of damage to apply.</param>
    public virtual void TakeDamage(float amount)
    {
        // Ignore damage if already considered dead or damage is non-positive
        if (currentHealth <= 0 || amount <= 0) return;

        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. Health: {currentHealth}/{maxHealth}");

        // Trigger player screen flash if applicable
        if (CompareTag("Player") && damageOverlay != null)
        {
            flashTimer = flashDuration;
            // Ensure alpha is reset correctly on new hit
            originalOverlayColor = damageOverlay.color; // Store current base color in case it changed
            originalOverlayColor.a = 0.4f; // Set target alpha (adjust as needed)
            damageOverlay.color = originalOverlayColor; // Apply immediately with alpha
        }

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Initiates the death sequence. Ensures health is 0, notifies recruiters,
    /// and calls PerformDeathAction. Can be overridden carefully, but usually
    /// overriding PerformDeathAction is preferred.
    /// </summary>
    public virtual void Die()
    {
        // Prevent multiple death calls / redundant actions
        if (currentHealth <= 0 && !gameObject.activeInHierarchy) return; // Quick check if already inactive
        if (currentHealth > 0) currentHealth = 0; // Ensure health is 0 if called externally

        Debug.Log($"'{name}' Die() sequence initiated...");

        // --- 1. Notify Recruiters (if applicable) ---
        if (myCollider != null && CompareTag(recruiterBotTag))
        {
            NotifyRecruitersOnDeath();
        }
        else if (CompareTag(recruiterBotTag)) // Log if tagged but no collider
        {
             Debug.LogWarning($"'{name}' tagged '{recruiterBotTag}' died but couldn't notify recruiters (missing Collider).");
        }

        // --- 2. Perform the actual death action (destroy, respawn, etc.) ---
        PerformDeathAction();
    }

    /// <summary>
    /// The specific action to take when death occurs (e.g., destroy object, reload scene, respawn).
    /// Meant to be overridden by derived classes (PlayerHealth, EnemyHealth, BossHealth).
    /// The base version reloads the current scene (intended primarily for player death).
    /// </summary>
    protected virtual void PerformDeathAction()
    {
        Debug.Log($"'{name}' executing base PerformDeathAction: Reloading scene.");
        // Check if the object still exists before trying to reload
        // (It might have been destroyed by an external system immediately after notification)
        if (this != null && gameObject != null)
        {
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // --- Helper Methods ---

    /// <summary>
    /// Sets the maximum health and optionally resets current health to the new maximum.
    /// Should ideally be called by derived classes in their Start() before base.Start().
    /// </summary>
    /// <param name="newMaxHealth">The new maximum health value.</param>
    /// <param name="setCurrentToMax">If true, current health is set to the new maximum.</param>
    public void SetMaxHealth(float newMaxHealth, bool setCurrentToMax = true)
    {
        maxHealth = Mathf.Max(0f, newMaxHealth); // Ensure max health isn't negative
        if (setCurrentToMax)
        {
            currentHealth = maxHealth;
        } else {
            // Optional: Clamp current health if max is reduced below current
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
         Debug.Log($"'{name}' max health set to {maxHealth}. Current health: {currentHealth}");
    }

    /// <summary>
    /// Gets the current health value.
    /// </summary>
    public float GetCurrentHealth() => currentHealth;

    /// <summary>
    /// Gets the maximum health value.
    /// </summary>
    public float GetMaxHealth() => maxHealth;

    /// <summary>
    /// Calculates the current health as a fraction of the maximum health (0.0 to 1.0).
    /// </summary>
    /// <returns>Health percentage (0.0 to 1.0), or 0 if maxHealth is zero.</returns>
    public float GetHealthPercent()
    {
        if (maxHealth <= 0)
        {
            return 0f; // Prevent division by zero
        }
        // Calculate and clamp the value between 0 and 1
        return Mathf.Clamp01(currentHealth / maxHealth);
    }

    // --- Internal Helpers ---

    /// <summary>
    /// Sets up the initial state of the damage overlay UI element.
    /// </summary>
    private void SetupDamageOverlay()
    {
        if (damageOverlay != null)
        {
            // Store the intended flash color (red with some transparency)
            // We apply it fully only during the flash in TakeDamage/Update
            originalOverlayColor = new Color(1f, 0f, 0f, 0.4f); // Target flash color & alpha
            // Start completely transparent
            damageOverlay.color = new Color(originalOverlayColor.r, originalOverlayColor.g, originalOverlayColor.b, 0f);
            damageOverlay.gameObject.SetActive(true); // Ensure it's active
        }
        else
        {
            Debug.LogWarning($"'{name}' (Player) is missing Damage Overlay Image assignment.", this);
        }
    }

    /// <summary>
    /// Finds all RecruiterDetection scripts in the scene and notifies them
    /// that this entity (identified by its collider) is being removed.
    /// </summary>
    protected void NotifyRecruitersOnDeath()
    {
        // Note: FindObjectsOfType can be slow if called frequently, but okay for infrequent death events.
        RecruiterDetection[] recruiters = FindObjectsOfType<RecruiterDetection>();

        if (recruiters.Length > 0 && myCollider != null)
        {
             Debug.Log($"'{name}' (Tag: {tag}) notifying {recruiters.Length} recruiter(s) about death...");
             foreach (RecruiterDetection recruiter in recruiters)
             {
                 // Tell each recruiter to remove this specific bot's collider
                 recruiter.ReportBotLeaving(myCollider);
             }
        }
        // Optional log if no recruiters found
        // else if (recruiters.Length == 0) { Debug.Log($"'{name}' died, no RecruiterDetection scripts found in scene to notify."); }
    }
}