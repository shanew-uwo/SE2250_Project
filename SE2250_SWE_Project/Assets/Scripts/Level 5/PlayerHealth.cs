using UnityEngine;

// Requires Level5RespawnManager to be on the same GameObject
[RequireComponent(typeof(Level5RespawnManager))]
public class PlayerHealth : Health // Inherit from our base Health
{
    private Level5RespawnManager respawnManager;

    public override void Start()
    {
        // You can call base.Start() to initialize health, collider, overlay etc.
        base.Start();
        respawnManager = GetComponent<Level5RespawnManager>();
        if (respawnManager == null)
        {
            Debug.LogError("PlayerHealth requires Level5RespawnManager component on the same GameObject!", this);
        }
        // Optional: Set player-specific max health if different from base default
        // SetMaxHealth(150f, true);
    }

    // Override the action performed when health reaches zero
    protected override void PerformDeathAction()
    {
        Debug.Log($"{name} died. Attempting to respawn.");

        if (respawnManager != null && respawnManager.CurrentCheckpoint != null)
        {
            // Restore health (optional, but usually desired on respawn)
            currentHealth = maxHealth; // Use protected fields from base class

            // Move player to checkpoint
            transform.position = respawnManager.CurrentCheckpoint.position;
            transform.rotation = respawnManager.CurrentCheckpoint.rotation;

            Debug.Log($"{name} respawned at checkpoint '{respawnManager.CurrentCheckpoint.name}'. Health restored.");

            // Optional: Reset velocity if using Rigidbody for player movement
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning($"{name} died but no valid checkpoint found via RespawnManager. Falling back to scene reload (or doing nothing).");
            // Fallback: Maybe reload scene if checkpoints fail? Or just log it.
            // base.PerformDeathAction(); // This would reload the scene
        }

        // DO NOT RELOAD SCENE HERE if respawn worked
    }
}