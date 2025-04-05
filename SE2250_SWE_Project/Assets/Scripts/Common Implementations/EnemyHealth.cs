using UnityEngine;

// Inherits from the base Health system
public class EnemyHealth : Health
{
    [Header("Enemy Specific Settings")]
    [SerializeField] private float enemyMaxHealth = 20f;
    // Add any other enemy-specific health properties here

    public override void Start()
    {
        // Set max health using the base class method BEFORE calling base.Start()
        // which initializes current health.
        SetMaxHealth(enemyMaxHealth, true);
        base.Start(); // Call the base Start method
    }

    // Override the death ACTION. Notification is handled by base Die().
    protected override void PerformDeathAction()
    {
        Debug.Log($"Enemy '{name}' has died. Destroying GameObject.");

        // Check if the object still exists before destroying
        if (this != null && gameObject != null)
        {
            Destroy(gameObject);
        }
        // DO NOT reload the scene for enemies
    }

    // Optional: Override TakeDamage if enemies react differently
    // public override void TakeDamage(float amount)
    // {
    //     base.TakeDamage(amount); // Call base logic first
    //     // Add enemy specific reaction to damage (e.g., play sound, particle effect)
    // }
}