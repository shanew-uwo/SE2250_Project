using UnityEngine;

public class Demon_Movement_Script : MonoBehaviour
{
    public Transform player;  // Assign the player's transform in the inspector
    public float speed = 5f;  // Movement speed
    public float stoppingDistance = 1.5f; // Minimum distance before stopping

    void Update()
    {

        if (player != null)
        {
            // Calculate direction to player
            Vector3 direction = (player.position - transform.position).normalized;

            // Check if the entity should move
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > stoppingDistance)
            {
                transform.position += direction * (speed * Time.deltaTime);
            }

            // Optionally make the enemy face the player
            transform.LookAt(player);
        }
    }
}