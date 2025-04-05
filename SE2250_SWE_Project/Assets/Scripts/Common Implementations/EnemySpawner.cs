using UnityEngine;
using System.Collections;
using Common_Implementations;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Setup")]
    [SerializeField] private GameObject _enemyPrefab; // Your Bot Prefab Variant
    [Tooltip("Tag the spawned enemies/bots with this tag.")]
    [SerializeField] private string spawnedEnemyTag = "Bot"; // Ensure this matches RecruiterDetection and PlayerAOEAttack

    [Header("Targeting for Spawned Enemies")]
    [Tooltip("Should spawned enemies move towards the Player or the Destination Object?")]
    [SerializeField] private EnemyMovementAI.TargetType enemyTargetType = EnemyMovementAI.TargetType.DestinationObject;
    [Tooltip("Assign the Pedestal (or target object) if Enemy Target Type is DestinationObject.")]
    [SerializeField] private Transform _targetDestination; // Assign HERE IN INSPECTOR!

    [Header("Spawn Timing")]
    [SerializeField] private float _minimumSpawnTime = 2.0f;
    [SerializeField] private float _maximumSpawnTime = 5.0f;

    private float _currentSpawnTimer;
    private Transform _playerTransform; // Cache player reference

    void Awake()
    {
        // --- KEEP THESE CHECKS ---
        // Adjust check based on selected target type
        if (enemyTargetType == EnemyMovementAI.TargetType.DestinationObject && _targetDestination == null)
        { Debug.LogError($"Spawner '{gameObject.name}' needs a Target Destination assigned (Enemy Target Type is DestinationObject)!", this); this.enabled = false; return; }

        if (_enemyPrefab == null)
        { Debug.LogError($"Spawner '{gameObject.name}' needs an Enemy Prefab assigned!", this); this.enabled = false; return; }

        // Find player once if needed
        if (enemyTargetType == EnemyMovementAI.TargetType.Player)
        {
            FindPlayer();
            if (_playerTransform == null)
            {
                Debug.LogError($"Spawner '{gameObject.name}' is set to spawn enemies targeting Player, but no GameObject with tag 'Player' found!", this);
                // Optionally disable, or enemies will search themselves
                 // this.enabled = false; return;
            }
        }

        // --- Set initial timer ---
        ResetSpawnTimer();
        Debug.Log($"Spawner '{gameObject.name}' initialized. First spawn in ~{_currentSpawnTimer:F1}s. Targeting: {enemyTargetType}");
    }

    void Update()
    {
        _currentSpawnTimer -= Time.deltaTime;

        if (_currentSpawnTimer <= 0f)
        {
            // Re-check for player just before spawning if targeting player
            // This handles cases where the player might have been destroyed/respawned
             if (enemyTargetType == EnemyMovementAI.TargetType.Player && _playerTransform == null)
             {
                 FindPlayer();
                 if (_playerTransform == null)
                 {
                    Debug.LogWarning($"Spawner '{gameObject.name}': Skipping spawn, cannot find Player target.");
                    ResetSpawnTimer(); // Still reset timer to avoid spamming checks
                    return; // Don't spawn if player not found
                 }
             }

            SpawnEnemy();
            ResetSpawnTimer();
        }
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = playerObj ? playerObj.transform : null;
    }

    public virtual void SpawnEnemy() // Keep virtual if you might subclass spawner
    {
        GameObject newEnemyGO = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        newEnemyGO.tag = spawnedEnemyTag; // Apply the specified tag

        EnemyMovementAI npcAI = newEnemyGO.GetComponent<EnemyMovementAI>();

        if (npcAI != null)
        {
            // Set the target based on the spawner's setting
            if (enemyTargetType == EnemyMovementAI.TargetType.Player)
            {
                npcAI.SetTarget(_playerTransform, EnemyMovementAI.TargetType.Player);
                 Debug.Log($"Spawner '{gameObject.name}' assigned Player target to spawned enemy '{newEnemyGO.name}'.", newEnemyGO);
            }
            else // DestinationObject
            {
                npcAI.SetTarget(_targetDestination, EnemyMovementAI.TargetType.DestinationObject);
                 Debug.Log($"Spawner '{gameObject.name}' assigned Destination target '{_targetDestination.name}' to spawned enemy '{newEnemyGO.name}'.", newEnemyGO);
            }
        }
        else
        {
            Debug.LogError($"Spawned enemy '{newEnemyGO.name}' from prefab '{_enemyPrefab.name}' is MISSING the required EnemyMovementAI script!", newEnemyGO);
        }

        // --- Optional: Configure other components ---
        // Example: Set health on the enemy if prefab doesn't have it set right
        Health enemyHealth = newEnemyGO.GetComponent<Health>();
        if (enemyHealth != null) {
             // enemyHealth.SetMaxHealth(50f); // Example: Override prefab health
        } else {
             Debug.LogWarning($"Spawned enemy '{newEnemyGO.name}' is MISSING a Health component!", newEnemyGO);
        }

        // Example: If using RangedEnemy_Lvl6, ensure it also gets the player ref IF needed
        RangedEnemy_Lvl6 rangedAttack = newEnemyGO.GetComponent<RangedEnemy_Lvl6>();
        if (rangedAttack != null && enemyTargetType == EnemyMovementAI.TargetType.Player) {
            rangedAttack.player = _playerTransform; // Assign player specifically for attack logic
             // OR modify RangedEnemy_Lvl6 to get target from EnemyMovementAI.GetCurrentTarget()
        }
    }

    private void ResetSpawnTimer()
    {
        _currentSpawnTimer = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }
}