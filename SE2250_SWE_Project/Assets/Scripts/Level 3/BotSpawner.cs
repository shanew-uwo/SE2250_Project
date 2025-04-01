using UnityEngine;

public class BotSpawner : SpawnerBase
{
 [Header("Bot Specific Settings")]
 [SerializeField] private bool randomizeSpawnPoints = true;

 protected override void Spawn()
 {
  if (prefabToSpawn == null)
  {
   Debug.LogWarning("No prefab assigned to spawn!");
   return;
  }

  if (spawnPoints.Count == 0)
  {
   Debug.LogWarning("No spawn points defined!");
   return;
  }

  int spawnIndex = randomizeSpawnPoints ? Random.Range(0, spawnPoints.Count) : currentSpawnCount % spawnPoints.Count;
  SpawnPoint point = spawnPoints[spawnIndex];

  Instantiate(prefabToSpawn, point.position, point.rotation);
  currentSpawnCount++;

  if (currentSpawnCount >= maxSpawnCount)
  {
   StopSpawning();
  }
 }

 // Additional bot-specific methods can be added here
}
