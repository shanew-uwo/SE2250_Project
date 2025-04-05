using UnityEngine;

// Ensure this inherits from the NEW Health script you created/updated
public class RangedEnemyHealth : Health
{
    [Header("Ranged Enemy Health")]
    [SerializeField] private float enemyMaxHealth = 20f; // Use a different name or make base maxHealth protected

    public override void Start()
    {
        // Use the public SetMaxHealth method from the base class
        // Pass 'true' to also set current health to max initially.
        SetMaxHealth(enemyMaxHealth, true);

        // Call the base Start method AFTER setting max health
        base.Start();
    }

    protected override void PerformDeathAction() // Override PerformDeathAction instead of Die
    {
        Debug.Log($"Ranged Enemy '{name}' has died. Destroying GameObject.");
        // Notification is handled by base Die() before this is called.

        // Check if object still exists (might be destroyed rapidly elsewhere)
        if (this != null && gameObject != null)
        {
            Destroy(gameObject);
        }
        // DO NOT call base.Die() here, that would cause recursion/double notification
        // DO NOT reload scene
    }
}