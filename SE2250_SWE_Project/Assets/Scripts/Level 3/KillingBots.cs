using UnityEngine;

public class KillingBots : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("The key to press for interaction.")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Tooltip("The maximum distance from the player to interact with a bot.")]
    [SerializeField] private float interactionRadius = 3.0f;

    [Tooltip("The tag assigned to the bot GameObjects.")]
    [SerializeField] private string botTag = "Bot"; // Make sure this matches your bots

    // Optional: Layer mask to only check specific layers for bots
    // [SerializeField] private LayerMask botLayerMask;

    void Update()
    {
        // Check if the interact key was pressed this frame
        if (Input.GetKeyDown(interactKey))
        {
            AttemptInteraction();
        }
    }

    void AttemptInteraction()
    {
        // Find all colliders within the interaction radius around the player
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, interactionRadius /*, botLayerMask */ ); // Uncomment LayerMask if using

        Collider closestBotCollider = null;
        float minDistance = float.MaxValue;

        // Iterate through all found colliders
        foreach (Collider col in nearbyColliders)
        {
            // Check if the collider belongs to a bot
            if (col.CompareTag(botTag))
            {
                // Calculate distance to this bot
                float distance = Vector3.Distance(transform.position, col.transform.position);

                // If this bot is closer than the previous closest one found
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBotCollider = col;
                }
            }
        }

        // If we found a bot within range
        if (closestBotCollider != null)
        {
            Debug.Log($"Player attempting to interact with closest bot: {closestBotCollider.name} at distance {minDistance:F2}m");

            // Try to get the EnemyHealth (or equivalent) script from the bot
            EnemyHealth enemyHealth = closestBotCollider.GetComponent<EnemyHealth>(); // Make sure the bot has this script

            if (enemyHealth != null)
            {
                // Tell the bot to die (which should handle notification and destruction)
                enemyHealth.Die();
                Debug.Log($"Player destroyed {closestBotCollider.name}!");
            }
            else
            {
                // Fallback: Destroy directly if no health script (Recruiter won't be notified!)
                Debug.LogWarning($"Bot {closestBotCollider.name} has no EnemyHealth script. Destroying directly (Recruiter NOT notified).");
                Destroy(closestBotCollider.gameObject);
            }
        }
        else
        {
            Debug.Log("Player pressed interact key, but no bots found within range.");
        }
    }

    // Optional: Visualize the interaction radius in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}