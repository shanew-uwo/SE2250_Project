using UnityEngine;
using System.Collections; // Add this if not present

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
        if (_targetDestination == null) { Debug.LogError($"Spawner '{gameObject.name}' needs a Target Destination assigned in the Inspector!", this); this.enabled = false; return; }
        if (_enemyPrefab == null) { Debug.LogError($"Spawner '{gameObject.name}' needs an Enemy Prefab assigned in the Inspector!", this); this.enabled = false; return; }

        // --- Set initial timer ---
        ResetSpawnTimer();
        Debug.Log($"Spawner '{gameObject.name}' initialized. First spawn in ~{_currentSpawnTimer}s.");
    }

    void Update()
    {
        _currentSpawnTimer -= Time.deltaTime;

        if (_currentSpawnTimer <= 0f)
        {
            SpawnEnemy();
            ResetSpawnTimer(); // Reset for the *next* spawn
            // Optional log: Debug.Log($"Spawner '{gameObject.name}' spawned enemy. Next spawn in ~{_currentSpawnTimer}s.");
        }
    }

    void SpawnEnemy()
    {
        GameObject newEnemyGO = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        NPCMovementAI npcAI = newEnemyGO.GetComponent<NPCMovementAI>();

        if (npcAI != null)
        {
            // This is the critical link: Pass the spawner's target to the new NPC
            npcAI.initialTargetDestination = _targetDestination;
            // Debug.Log($"Spawner '{gameObject.name}' assigned target '{_targetDestination.name}' to spawned enemy '{newEnemyGO.name}'.", newEnemyGO);
        }
        else
        {
            // Prefab is missing the required script!
            Debug.LogError($"Spawned enemy '{newEnemyGO.name}' from prefab '{_enemyPrefab.name}' is MISSING the NPCMovementAI script!", newEnemyGO);
        }
    }

    private void ResetSpawnTimer()
    {
        _currentSpawnTimer = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }
}