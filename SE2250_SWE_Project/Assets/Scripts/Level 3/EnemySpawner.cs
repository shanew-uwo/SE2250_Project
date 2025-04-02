using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Setup")]
    [SerializeField]
    private GameObject _enemyPrefab; // Your Bot Prefab Variant

    [Tooltip("The object the spawned enemies should move towards.")]
    [SerializeField]
    private Transform _targetDestination; // Assign the Pedestal here!

    [Header("Spawn Timing")]
    [SerializeField]
    private float _minimumSpawnTime = 2.0f;
    [SerializeField]
    private float _maximumSpawnTime = 5.0f;

    private float _currentSpawnTimer; // Renamed for clarity

    void Awake()
    {
        if (_targetDestination == null) { Debug.LogError($"Spawner '{gameObject.name}' needs a Target Destination!", this); this.enabled = false; return; }
        if (_enemyPrefab == null) { Debug.LogError($"Spawner '{gameObject.name}' needs an Enemy Prefab!", this); this.enabled = false; return; }
        // Set initial timer immediately
        ResetSpawnTimer();
        Debug.Log($"Spawner '{gameObject.name}' initialized. First spawn in ~{_currentSpawnTimer}s.");
    }

    void Update()
    {
        _currentSpawnTimer -= Time.deltaTime;

        // Check if timer expired
        if (_currentSpawnTimer <= 0f)
        {
            // Spawn ONE enemy
            SpawnEnemy();

            // Immediately reset timer for the NEXT spawn
            ResetSpawnTimer();
            // Optional log: Debug.Log($"Spawner '{gameObject.name}' spawned enemy. Next spawn in ~{_currentSpawnTimer}s.");
        }
    }

    void SpawnEnemy()
    {
        // No need for null checks here if Awake passed
        GameObject newEnemyGO = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        NPCMovementAI npcAI = newEnemyGO.GetComponent<NPCMovementAI>();
        if (npcAI != null)
        {
            npcAI.initialTargetDestination = _targetDestination;
            // Debug.Log($"Spawner '{gameObject.name}' assigned target '{_targetDestination.name}' to spawned enemy '{newEnemyGO.name}' with AI script.", newEnemyGO); // Keep if needed
        } else {
            Debug.LogError($"Spawned enemy '{newEnemyGO.name}' is MISSING the NPCMovementAI script!", newEnemyGO);
        }
    }

    // Renamed from SetTimeUntilSpawn
    private void ResetSpawnTimer()
    {
        _currentSpawnTimer = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }
}