using UnityEngine;
using System.Collections.Generic;

public abstract class SpawnerBase : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Vector3 position;
        public Quaternion rotation = Quaternion.identity;
    }

    [Header("Spawn Settings")]
    [SerializeField] protected GameObject prefabToSpawn;
    [SerializeField] protected List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    [SerializeField] protected float spawnInterval = 1f;
    [SerializeField] protected int maxSpawnCount = 10;

    protected int currentSpawnCount = 0;
    protected float spawnTimer = 0f;
    protected bool isSpawning = true;

    protected virtual void Update()
    {
        if (!isSpawning || spawnPoints.Count == 0 || currentSpawnCount >= maxSpawnCount)
            return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            Spawn();
        }
    }

    protected abstract void Spawn();

    public void AddSpawnPoint(Vector3 position, Quaternion rotation)
    {
        spawnPoints.Add(new SpawnPoint { position = position, rotation = rotation });
    }

    public void ClearSpawnPoints()
    {
        spawnPoints.Clear();
    }

    public void StartSpawning()
    {
        isSpawning = true;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void SetSpawnInterval(float interval)
    {
        spawnInterval = Mathf.Max(0.1f, interval);
    }
}