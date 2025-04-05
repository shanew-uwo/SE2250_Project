using UnityEngine;

// Add RequireComponent to ensure EnemyMovementAI exists
[RequireComponent(typeof(Common_Implementations.EnemyMovementAI))]
public class RangedEnemy_Lvl6 : MonoBehaviour
{
    // Remove the public player field:
    // public Transform player;

    public GameObject projectilePrefab;
    // Removed moveSpeed - let EnemyMovementAI handle movement
    // public float moveSpeed = 2f;
    public float projectileSpeed = 5f;
    public float fireRate = 1f;
    [Tooltip("How far from the enemy's center the projectile should spawn.")]
    public float projectileSpawnOffset = 1f; // Renamed from spawnDistance for clarity
    [Tooltip("Maximum distance at which the enemy will fire.")]
    public float fireDistance = 15f;

    private float fireCooldown;
    private Common_Implementations.EnemyMovementAI movementAI; // Reference to the movement component
    public Transform player;

    void Awake() // Use Awake to get references reliably
    {
        movementAI = GetComponent<Common_Implementations.EnemyMovementAI>();
        if (movementAI == null)
        {
            Debug.LogError($"RangedEnemy_Lvl6 on {name} requires an EnemyMovementAI component!", this);
            enabled = false; // Disable script if dependency is missing
        }
         if (projectilePrefab == null)
         {
             Debug.LogError($"RangedEnemy_Lvl6 on {name} is missing the Projectile Prefab!", this);
             enabled = false;
         }
    }

    void Update()
    {
        // Get the current target FROM the movement AI
        Transform currentTarget = movementAI.GetCurrentTarget();

        if (currentTarget == null) return; // Don't do anything if no target

        // Calculate direction and distance to the CURRENT target
        Vector3 direction = (currentTarget.position - transform.position); // Don't normalize yet for distance check
        float distance = direction.magnitude; // Use magnitude for distance

        // Normalize direction AFTER distance check for aiming/movement checks
        direction.Normalize();

        // --- Movement is now handled by EnemyMovementAI ---
        // Removed movement logic from here:
        // Vector3 lookTarget = new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z);
        // transform.LookAt(lookTarget); // EnemyMovementAI handles rotation
        // if (distance > someStopDistance) { /* EnemyMovementAI handles stopping */ }

        // --- Firing Logic ---
        if (distance <= fireDistance)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                // Pass the calculated direction to the target
                FireProjectile(direction);
                fireCooldown = 1f / fireRate;
            }
        }
        else
        {
            // Reset cooldown if target moves out of range? Optional.
             fireCooldown = 1f / fireRate; // Or some other value like 0.1f
        }
    }

    void FireProjectile(Vector3 direction)
    {
        // Calculate spawn position slightly in front of the enemy
        Vector3 spawnPos = transform.position + direction * projectileSpawnOffset;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction)); // Aim projectile

        EnemyProjectile_Lvl6 projectileScript = proj.GetComponent<EnemyProjectile_Lvl6>();
        if (projectileScript != null)
        {
            projectileScript.Launch(direction, projectileSpeed);
        }
        else
        {
            // Log error if the specific script is missing, still try Rigidbody
            Debug.LogError($"Projectile script 'EnemyProjectile_Lvl6' not found on prefab '{projectilePrefab.name}'. Trying Rigidbody launch.", proj);
             Rigidbody rb = proj.GetComponent<Rigidbody>();
             if(rb != null) {
                  rb.velocity = direction * projectileSpeed;
             } else {
                 Debug.LogError($"Projectile prefab '{projectilePrefab.name}' also missing Rigidbody.", proj);
             }
        }
         // Removed redundant Launch call
         // projectileScript.Launch(direction, projectileSpeed);
    }

    // Remove the setPlayer method as it's no longer needed
    // public void setPlayer(Transform player) { ... }
    public void setPlayer(Transform p0)
    {
        throw new System.NotImplementedException();
    }
}