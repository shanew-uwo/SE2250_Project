using UnityEngine;
using System.Collections;
// Add namespace if used by EnemyMovementAI
// using Common_Implementations;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Setup")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private string spawnedEnemyTag = "Bot";

    [Header("Spawner Settings")]
    [Tooltip("Assign the object enemies should return to when idle (e.g., Pedestal). Leave None to have them stand still or return to spawn.")]
    [SerializeField] private Transform _targetDestination; // This is the IDLE destination

    [Header("Spawn Timing")]
    [SerializeField] private float _minimumSpawnTime = 2.0f;
    [SerializeField] private float _maximumSpawnTime = 5.0f;

    private float _currentSpawnTimer;
    // No longer need spawner to track player or target type
    // private Transform _playerTransform;
    // private EnemyMovementAI.TargetType enemyTargetType;

    void Awake()
    {
        // Simplified Checks
        if (_targetDestination == null)
        {
            // This is now more informational, as AI can handle null destination
            Debug.Log($"Spawner '{gameObject.name}' does not have a Target Destination assigned. Idle enemies may stand still or use spawn point.", this);
        }
        if (_enemyPrefab == null)
        {
            Debug.LogError($"Spawner '{gameObject.name}' needs an Enemy Prefab assigned!", this); this.enabled = false; return;
        }
        if (_enemyPrefab.GetComponent<EnemyMovementAI>() == null) // Or Namespace.EnemyMovementAI
        {
             Debug.LogError($"Enemy Prefab assigned to Spawner '{gameObject.name}' is MISSING the EnemyMovementAI script!", _enemyPrefab); this.enabled = false; return;
        }

        ResetSpawnTimer();
        Debug.Log($"Spawner '{gameObject.name}' initialized. First spawn in ~{_currentSpawnTimer:F1}s.");
    }

    void Update()
    {
        _currentSpawnTimer -= Time.deltaTime;
        if (_currentSpawnTimer <= 0f)
        {
            SpawnEnemy();
            ResetSpawnTimer();
        }
    }

    // Removed FindPlayer

    public virtual void SpawnEnemy()
    {
        GameObject newEnemyGO = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        newEnemyGO.tag = spawnedEnemyTag;

        EnemyMovementAI npcAI = newEnemyGO.GetComponent<EnemyMovementAI>(); // Or Namespace.EnemyMovementAI

        // --- Configure the newly spawned enemy ---
        if (npcAI != null)
        {
            // *** CHANGE HERE: Call AssignFixedDestination ***
            // Pass the pedestal (or null if none assigned) to the AI.
            // The AI will use this when the player is NOT detected.
            npcAI.AssignFixedDestination(_targetDestination);
            // *** END CHANGE ***
        }
        else
        {
            Debug.LogError($"Spawned enemy '{newEnemyGO.name}' from prefab '{_enemyPrefab.name}' is MISSING the required EnemyMovementAI script!", newEnemyGO);
        }

        // --- Optional: Configure other components ---
        Health enemyHealth = newEnemyGO.GetComponent<Health>();
        if (enemyHealth == null) {
             Debug.LogWarning($"Spawned enemy '{newEnemyGO.name}' is MISSING a Health component!", newEnemyGO);
        }
        // Removed RangedEnemy_Lvl6 specific code - that script gets target from EnemyMovementAI now
    }

    private void ResetSpawnTimer()
    {
        _currentSpawnTimer = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }
}