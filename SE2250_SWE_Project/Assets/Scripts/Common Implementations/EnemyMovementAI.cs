// Keep the namespace if you are using it
// namespace Common_Implementations
// {
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyMovementAI : MonoBehaviour
{
    // Removed TargetType enum - logic is now dynamic based on detection
    // public enum TargetType { DestinationObject, Player }

    [Header("Detection & Idle Target")]
    [SerializeField] private float detectionRadius = 10f; // Player detection radius
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Assign the fixed destination (e.g., Pedestal) the enemy returns to when idle.")]
    [SerializeField] private Transform fixedDestination; // Can be set in Prefab OR by Spawner

    [Header("Movement")]
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float stoppingDistance = 1.5f; // Distance to stop from target
    [SerializeField] private float rotationSpeed = 120f; // Degrees per second

    // Internal State
    private Transform currentTarget; // What the AI is actively moving towards NOW
    private Transform playerTransform; // Cached reference to the player
    private bool isPlayerDetected = false; // State flag for logging/debugging
    private Vector3 initialPosition; // Store initial position as ultimate fallback

    // Optional NavMeshAgent: private NavMeshAgent agent;

    void Awake()
    {
        initialPosition = transform.position;
        // Optional NavMeshAgent setup
    }

    void Start()
    {
        FindPlayer();
        // Set initial target based on whether player is immediately detected or not
        // This ensures correct starting behavior even if spawned near player
        HandleDetectionAndTargetSwitch(); // Call this to set initial target
        if (currentTarget == null && fixedDestination == null)
        {
             Debug.LogWarning($"'{name}' has no Player detected and no Fixed Destination. It will stand still.", this);
        }
        else if (currentTarget == null && fixedDestination != null)
        {
            // If player wasn't detected, explicitly set idle target
             currentTarget = fixedDestination;
             Debug.Log($"'{name}' Starting Idle, Target: {currentTarget.name}", this);
        }

    }

    void Update()
    {
        // 1. Make sure we have a player reference
        if (playerTransform == null) FindPlayer();

        // 2. Decide who to target THIS FRAME
        HandleDetectionAndTargetSwitch();

        // 3. Move towards the decided target
        PerformMovement();
    }

    void HandleDetectionAndTargetSwitch()
    {
        // Default to idle target if player doesn't exist
        if (playerTransform == null)
        {
            if (currentTarget != fixedDestination) // Only switch if not already targeting destination
            {
                Debug.Log($"'{name}' Player not found. Setting target to fixed destination.", this);
                currentTarget = fixedDestination; // Fallback to fixed destination
                 isPlayerDetected = false;
            }
            return; // Nothing more to do if player doesn't exist
        }

        // Calculate distance
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Check detection range
        if (distanceToPlayer <= detectionRadius)
        {
            // Player is IN range - TARGET PLAYER
            if (currentTarget != playerTransform) // Only switch if not already targeting player
            {
                 if (!isPlayerDetected) Debug.Log($"'{name}' Detected Player '{playerTransform.name}'. Switching target.", this);
                 currentTarget = playerTransform;
                 isPlayerDetected = true;
            }
        }
        else
        {
            // Player is OUT of range - TARGET FIXED DESTINATION (if available)
            if (currentTarget != fixedDestination) // Only switch if not already targeting destination
            {
                 if (isPlayerDetected) Debug.Log($"'{name}' Lost Player '{playerTransform.name}'. Returning to fixed destination.", this);
                 currentTarget = fixedDestination; // Target the pedestal (or null if not set)
                 isPlayerDetected = false;
                 if (currentTarget == null) {
                     // If no fixed destination, maybe target initial spawn point?
                     // This part needs decision: Stand still or return to spawn?
                     // For now, it will stand still if fixedDestination is null.
                     // Option: Create a temporary target at initialPosition if needed.
                     Debug.Log($"'{name}' Lost Player, no fixed destination. Standing still.", this);
                 }
            }
        }
    }


    void PerformMovement()
    {
        // --- Movement Logic ---
        if (currentTarget != null) // Only move if there IS a target
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            if (distance > stoppingDistance)
            {
                MoveTowardsTarget();
                RotateTowardsTarget();
            }
            else // Within stopping distance
            {
                // Keep rotating only if actively targeting the player
                if (isPlayerDetected && currentTarget == playerTransform)
                {
                    RotateTowardsTarget();
                }
                // If idle at destination, stop rotating
            }
        }
        // If currentTarget is null, do nothing (stand still)
    }


    void MoveTowardsTarget()
    {
        if (currentTarget == null) return; // Safety check
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * (speed * Time.deltaTime);
    }

    void RotateTowardsTarget()
    {
        if (currentTarget == null) return; // Safety check
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            // Log less frequently? Only if playerTransform was previously not null?
            if (playerTransform != null) // Log only when player is lost
                 Debug.LogWarning($"Enemy '{name}': Lost player reference (Tag='{playerTag}').", this);
            playerTransform = null;
        }
    }

    // --- Method for Spawner to set the FIXED destination ---
    // Changed name to avoid conflict/confusion with old SetTarget
    public void AssignFixedDestination(Transform destination)
    {
        fixedDestination = destination;
        if (fixedDestination == null)
        {
             Debug.LogWarning($"'{name}' was assigned a NULL fixed destination by the Spawner. It may stand still when idle.", this);
        }
        else {
            Debug.Log($"'{name}' Fixed Destination assigned: {fixedDestination.name}", this);
            // If not currently chasing player, immediately set target to this destination
            if (!isPlayerDetected) {
                currentTarget = fixedDestination;
                Debug.Log($"'{name}' Target immediately set to fixed destination after assignment.", this);
            }
        }
    }

    // Removed the old SetTarget(Transform, TargetType) method
    // public void SetTarget(...) {}

    // Removed SetCurrentTarget - now handled internally by HandleDetectionAndTargetSwitch
    // private void SetCurrentTarget(...) {}

    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }

     void OnDrawGizmosSelected()
    {
        // Detection radius
        Gizmos.color = isPlayerDetected ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Line to active target
        if (currentTarget != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }

        // Marker/Line for fixed destination
        if (fixedDestination != null) {
             Gizmos.color = Color.blue;
             Gizmos.DrawWireSphere(fixedDestination.position, 0.5f);
             // Optionally draw line only if different from current target
             if (currentTarget != fixedDestination) {
                Gizmos.DrawLine(transform.position, fixedDestination.position);
             }
        }
    }
}
// } // End namespace if using