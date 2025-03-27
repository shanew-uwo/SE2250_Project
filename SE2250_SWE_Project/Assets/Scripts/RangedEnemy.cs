using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public Transform player;
    public GameObject projectilePrefab;

    public float moveSpeed = 2f;
    public float projectileSpeed = 20f;
    public float fireRate = 1f; // 1 shot per second
    public float spawnDistance = 1f;
    public float fireDistance = 15f;

    private float fireCooldown;

    void Update()
    {
        if (player == null) return;

        // Direction and distance to the player
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(player.position, transform.position);

        // Move toward player
        transform.position += direction * moveSpeed * Time.deltaTime;

        // If within range, shoot at a fixed rate
        if (distance <= fireDistance)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                FireProjectile(direction);
                fireCooldown = 1f / fireRate; // Reset cooldown
            }
        }
    }

    void FireProjectile(Vector3 direction)
    {
        Vector3 spawnPos = transform.position + direction * spawnDistance;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }
    }
}