using System.Collections;
using UnityEngine;

// Add RequireComponent to ensure EnemyMovementAI exists
// If EnemyMovementAI is still in a namespace: using Common_Implementations;
[RequireComponent(typeof(EnemyMovementAI))] // Or Common_Implementations.EnemyMovementAI
public class RangedEnemy_Lvl6 : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float fireRate = 1f;
    [Tooltip("How far from the enemy's center the projectile should spawn.")]
    public float projectileSpawnOffset = 1f;
    [Tooltip("Maximum distance at which the enemy will fire.")]
    public float fireDistance = 15f;

    private float fireCooldown;
    private EnemyMovementAI movementAI; // Or Common_Implementations.EnemyMovementAI
    public Transform player;

    // --- REMOVE THESE LINES ---
    // public Transform player;
    // --------------------------

    void Awake()
    {
        movementAI = GetComponent<EnemyMovementAI>(); // Or Common_Implementations.EnemyMovementAI
        if (movementAI == null)
        {
            Debug.LogError($"RangedEnemy_Lvl6 on {name} requires an EnemyMovementAI component!", this);
            enabled = false;
        }
        if (projectilePrefab == null)
        {
            Debug.LogError($"RangedEnemy_Lvl6 on {name} is missing the Projectile Prefab!", this);
            enabled = false;
        }
    }
    
    void Start()
    {
        StartCoroutine(FindPlayerDelayed());
    }

    IEnumerator FindPlayerDelayed()
    {
        yield return new WaitForEndOfFrame(); // Wait until everything in Start() has run
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found after delay.");
        }
    }

    void Update()
    {
        Transform currentTarget = movementAI.GetCurrentTarget();
        if (currentTarget == null) return;

        Vector3 direction = (currentTarget.position - transform.position);
        float distance = direction.magnitude;
        direction.Normalize(); // Normalize AFTER distance check

        if (distance <= fireDistance)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                FireProjectile(direction);
                fireCooldown = 1f / fireRate;
            }
        }
        // Removed redundant else block, cooldown handled above
    }

    void FireProjectile(Vector3 direction)
    {
        Vector3 spawnPos = transform.position + direction * projectileSpawnOffset;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction));

        EnemyProjectile_Lvl6 projectileScript = proj.GetComponent<EnemyProjectile_Lvl6>();
        if (projectileScript != null)
        {
            projectileScript.Launch(direction, projectileSpeed);
        }
        else
        {
            Debug.LogError($"Projectile script 'EnemyProjectile_Lvl6' not found on prefab '{projectilePrefab.name}'. Trying Rigidbody launch.", proj);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if(rb != null) {
                rb.linearVelocity = direction * projectileSpeed;
            } else {
                Debug.LogError($"Projectile prefab '{projectilePrefab.name}' also missing Rigidbody.", proj);
            }
        }
    }

    // --- REMOVE THIS METHOD ---
    // public void setPlayer(Transform p0)
    // {
    //     throw new System.NotImplementedException();
    // }
    // --------------------------
}