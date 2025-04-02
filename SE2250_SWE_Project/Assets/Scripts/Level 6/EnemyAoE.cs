using System.Collections;
using UnityEngine;

public class EnemyAoE : MonoBehaviour
{
    public float aoeRadius = 50f;      // Radius of AoE attack
    public float aoeDamage = 20f;     // Damage dealt by AoE attack
    public float attackInterval = 3f; // Time in seconds between AoE attacks

    private float attackTimer = 0f;   // Timer to track attack intervals

    private void Update()
    {
        // Update the attack timer
        attackTimer -= Time.deltaTime;

        // If the timer reaches 0, perform the AoE attack
        if (attackTimer <= 0f)
        {
            PerformAoEAttack();
            attackTimer = attackInterval; // Reset the attack timer
        }
    }

    private void PerformAoEAttack()
    {
        GetComponent<BossAOEVisual>().StartAOEEffect();
        // Get all colliders within the AoE radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aoeRadius);

        foreach (Collider collider in hitColliders)
        {
            // Check if the collider is an enemy, player, or anything that should take damage
            if (collider.CompareTag("Player"))
            {
                // Apply damage to the target (you can extend this to other types of objects)
                Health targetHealth = collider.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(aoeDamage);
                }
            }
        }

        // Optionally, visualize the AoE area (for debugging or visual effects)
        Debug.DrawRay(transform.position, Vector3.up, Color.red, 1f);
    }

    // You could also visualize the AoE range with a gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}