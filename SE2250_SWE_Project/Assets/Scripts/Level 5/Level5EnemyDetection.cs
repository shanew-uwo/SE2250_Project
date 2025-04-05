using UnityEngine;

public class Level5EnemyDetection : MonoBehaviour
{
    public float detectionRadius = 10f;
    public string playerTag = "Player";

    private RangedEnemy_Lvl6 rangedEnemy;
    private Transform player;

    void Start()
    {
        rangedEnemy = GetComponent<RangedEnemy_Lvl6>();
        if (rangedEnemy == null)
        {
            Debug.LogError("RangedEnemy_Lvl6 script not found on this GameObject.");
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player GameObject with tag 'Player' not found.");
        }
    }

    void Update()
    {
        if (player == null || rangedEnemy == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // --- DELETE THESE OBSOLETE LINES ---
        // if (distance <= detectionRadius && rangedEnemy.player == null)
        // {
        //     rangedEnemy.setPlayer(player);
        // }
        // else if (distance > detectionRadius && rangedEnemy.player != null)
        // {
        //     rangedEnemy.setPlayer(null);
        // }
        // --- END DELETED LINES ---
    }

    // --- CORRECTED METHOD SIGNATURE ---
    void OnDrawGizmosSelected() // Added 'void' and '()'
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}