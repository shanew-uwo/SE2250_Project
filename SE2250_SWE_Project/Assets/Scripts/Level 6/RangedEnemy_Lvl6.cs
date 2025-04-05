using UnityEngine;

public class RangedEnemy_Lvl6 : MonoBehaviour
{
    public Transform player;
    public GameObject projectilePrefab;
    public float moveSpeed = 2f;
    public float projectileSpeed = 5f;
    public float fireRate = 1f;
    public float spawnDistance = 1f;
    public float fireDistance = 15f;

    private float fireCooldown;

    void Update()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(player.position, transform.position);

        Vector3 lookTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookTarget);
        
        if (distance > spawnDistance + 0.5f)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        if (distance <= fireDistance)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                FireProjectile(direction);
                fireCooldown = 1f / fireRate;
            }
        }
    }

    void FireProjectile(Vector3 direction)
    {
        Vector3 spawnPos = transform.position + direction * spawnDistance;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        EnemyProjectile_Lvl6 projectileScript = proj.GetComponent<EnemyProjectile_Lvl6>();
        if (projectileScript != null)
        {
            projectileScript.Launch(direction, projectileSpeed);
        }
        else
        {
            Debug.LogWarning("Projectile script not found on projectile prefab.");
        }
        
        projectileScript.Launch(direction, projectileSpeed);
    }

    public void setPlayer(Transform player)
    {
        this.player = player;
    }
}