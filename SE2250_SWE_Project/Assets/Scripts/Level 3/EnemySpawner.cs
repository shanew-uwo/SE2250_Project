using UnityEngine;
using System.Collections; // Required for Random.Range, though implicitly used by Update loop timing

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Setup")]
    [SerializeField]
    private GameObject _enemyPrefab; // Your Bot Prefab Variant

    [Tooltip("The object the spawned enemies should move towards.")]
    [SerializeField]
    private Transform _targetDestination; // Assign the Pedestal (or target object) HERE IN THE INSPECTOR!

    [Header("Spawn Timing")]
    [SerializeField]
    private float _minimumSpawnTime = 2.0f;
    [SerializeField]
    private float _maximumSpawnTime = 5.0f;

    private float _currentSpawnTimer; // Now used for countdown

    void Awake()
    {
        // --- KEEP THESE CHECKS --- they are crucial! ---
        // These errors indicate a setup problem that prevents the spawner from functioning.
        if (_targetDestination == null) {
            Debug.LogError($"Spawner '{gameObject.name}' needs a Target Destination assigned in the Inspector!", this);
            this.enabled = false;
            return;
        }
        if (_enemyPrefab == null) {
            Debug.LogError($"Spawner '{gameObject.name}' needs an Enemy Prefab assigned in the Inspector!", this);
            this.enabled = false;
            return;
        }

        // --- Set initial timer ---
        ResetSpawnTimer();
        // Debug.Log($"Spawner '{gameObject.name}' initialized. First spawn in ~{_currentSpawnTimer}s."); // Removed log
    }

    void Update()
    {
        _currentSpawnTimer -= Time.deltaTime;

        if (_currentSpawnTimer <= 0f)
        {
            SpawnEnemy();
            ResetSpawnTimer(); // Reset for the *next* spawn
            // Optional log: // Debug.Log($"Spawner '{gameObject.name}' spawned enemy. Next spawn in ~{_currentSpawnTimer}s."); // Removed log
        }
    }

    void SpawnEnemy()
    {
        if (_enemyPrefab == null || _targetDestination == null) {
             return;
        }

        GameObject newEnemyGO = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        NPCMovementAI npcAI = newEnemyGO.GetComponent<NPCMovementAI>();

        if (npcAI != null)
        {
            npcAI.initialTargetDestination = _targetDestination;
        }
    }

    private void ResetSpawnTimer()
    {
        _currentSpawnTimer = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }
}