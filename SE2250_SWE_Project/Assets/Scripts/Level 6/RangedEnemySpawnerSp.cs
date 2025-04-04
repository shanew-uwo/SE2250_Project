using UnityEngine;

public class RangedEnemySpawner : MonoBehaviour
{
    [Header("Enemy Spawning")]
    public GameObject rangedEnemyPrefab;
    public Transform player; // Target for all enemies
    public float spawnInterval = 5f;
    public int maxEnemies = 5;
    public float spawnRadius = 10f;

    private float spawnTimer = 0f;

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        // Count current enemies in the scene
        int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (spawnTimer <= 0f && currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (rangedEnemyPrefab == null || player == null)
        {
            Debug.LogWarning("Spawner is missing references.");
            return;
        }

        // Random position around the spawner within a circle
        Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(offset2D.x, 0, offset2D.y);

        GameObject enemy = Instantiate(rangedEnemyPrefab, spawnPos, Quaternion.identity);
        enemy.tag = "Enemy"; // Make sure enemy prefab is tagged properly

        // Assign the player to the enemy script
        RangedEnemy_Lvl6 script = enemy.GetComponent<RangedEnemy_Lvl6>();
        if (script != null)
        {
            script.player = player;
        }
        else
        {
            Debug.LogWarning("Spawned enemy does not have RangedEnemy_Lvl6 script.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}