using UnityEngine;

// Flexible movement script for NPCs/Enemies
namespace Common_Implementations
{
    [RequireComponent(typeof(Collider))] // Needed for interaction/detection
// Optional: [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMovementAI : MonoBehaviour
    {
        public enum TargetType { DestinationObject, Player }

        [Header("Targeting")]
        [SerializeField] private TargetType initialTargetType = TargetType.DestinationObject;
        [SerializeField] private string playerTag = "Player";
        [Tooltip("Assign the fixed destination (e.g., Pedestal) if Initial Target Type is DestinationObject.")]
        [SerializeField] private Transform fixedDestination;

        [Header("Movement")]
        [SerializeField] private float speed = 3.5f;
        [SerializeField] private float stoppingDistance = 1.5f; // Distance to stop from target
        [SerializeField] private float rotationSpeed = 120f; // Degrees per second

        // Internal State
        private Transform currentTarget;
        private Transform playerTransform;
        // Optional NavMeshAgent: private NavMeshAgent agent;

        void Awake()
        {
            // Optional NavMeshAgent setup:
            // agent = GetComponent<NavMeshAgent>();
            // if (agent != null)
            // {
            //     agent.speed = speed;
            //     agent.stoppingDistance = stoppingDistance;
            // }
        }

        void Start()
        {
            FindPlayer(); // Attempt to find player initially
            SetInitialTarget();
        }

        void Update()
        {
            // If targeting player, ensure we still have a reference (player might respawn)
            if (initialTargetType == TargetType.Player && playerTransform == null)
            {
                FindPlayer();
                SetCurrentTarget(playerTransform); // Update current target if found
            }

            // --- Movement Logic ---
            if (currentTarget != null)
            {
                float distance = Vector3.Distance(transform.position, currentTarget.position);

                if (distance > stoppingDistance)
                {
                    // Optional NavMeshAgent movement:
                    // if (agent != null && agent.isOnNavMesh)
                    // {
                    //    agent.SetDestination(currentTarget.position);
                    // }
                    // else // Simple MoveTowards fallback
                    // {
                    MoveTowardsTarget();
                    RotateTowardsTarget();
                    // }
                }
                else
                {
                    // Optional NavMeshAgent stop:
                    // if (agent != null && agent.isOnNavMesh)
                    // {
                    //     if(!agent.isStopped) agent.isStopped = true; // Stop agent
                    // }

                    // Optional: Play idle animation or look around
                    RotateTowardsTarget(); // Keep looking at target even when stopped
                }
            }
            // --- End Movement Logic ---
        }

        void MoveTowardsTarget()
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            // Ignore Y difference for ground movement if not using NavMesh
            direction.y = 0;
            transform.position += direction * (speed * Time.deltaTime);
        }

        void RotateTowardsTarget()
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            // Ignore Y difference for rotation to prevent tilting
            direction.y = 0;
            if (direction != Vector3.zero) // Prevent LookRotation error if target is directly above/below
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }
        }


        // --- Target Management ---

        void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogWarning($"Enemy '{name}': Cannot find GameObject with tag '{playerTag}'.", this);
                playerTransform = null; // Ensure it's null if not found
            }
        }

        void SetInitialTarget()
        {
            if (initialTargetType == TargetType.Player)
            {
                SetCurrentTarget(playerTransform); // Use cached player reference
            }
            else // DestinationObject
            {
                if(fixedDestination == null)
                {
                    Debug.LogError($"Enemy '{name}' is set to target DestinationObject, but 'Fixed Destination' is not assigned!", this);
                }
                SetCurrentTarget(fixedDestination);
            }
        }

        // Allows external scripts (like Spawner) to set the target AFTER spawn
        public void SetTarget(Transform newTarget, TargetType type)
        {
            initialTargetType = type; // Update type if needed
            if(type == TargetType.DestinationObject)
            {
                fixedDestination = newTarget;
                SetCurrentTarget(fixedDestination);
            }
            else // Player
            {
                playerTransform = newTarget; // Assume newTarget is the player if type is Player
                SetCurrentTarget(playerTransform);
            }

            // Optional NavMeshAgent update:
            // if (agent != null) agent.isStopped = false; // Resume movement if stopped
        }

        private void SetCurrentTarget(Transform target)
        {
            currentTarget = target;
            Debug.Log($"Enemy '{name}' target set to: {(target ? target.name : "null")}");

            // Optional NavMeshAgent reset path:
            // if (agent != null)
            // {
            //     agent.isStopped = (target == null); // Stop if no target
            //     if (target != null) agent.SetDestination(target.position);
            // }
        }

        // Public getter for other scripts (like ranged attack) if needed
        public Transform GetCurrentTarget()
        {
            return currentTarget;
        }
    }
}