using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent
using System.Collections; // Required for Coroutines

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCMovementAI : MonoBehaviour
{
    public enum NPCState
    {
        Initializing,
        RunningToInitialTarget,
        Wandering_Deciding,
        Wandering_Moving
        // Consider adding an Idle/Error state if initialization fails badly
    }

    [Header("Movement Target")]
    public Transform initialTargetDestination; // THIS IS SET BY THE SPAWNER

    [Header("Movement Speeds")]
    public float runningSpeed = 5.0f;
    public float walkingSpeed = 2.0f;

    [Header("Wandering Behaviour")]
    public float wanderRadius = 5.0f;
    public float wanderMinWaitTime = 2.0f;
    public float wanderMaxWaitTime = 5.0f;

    [Header("Animation")]
    public string runParamName = "isRunning";
    public string walkParamName = "isWalking";

    [Header("Agent Settings (Read Only - Check Component)")]
    [SerializeField] private bool agentUpdateRotation = true; // Informational
    [SerializeField] private bool animatorApplyRootMotion = false; // Informational

    private NavMeshAgent agent;
    private Animator animator;
    private NPCState currentState;
    private float wanderWaitTimer;
    private Vector3 initialTargetPosition; // Store the position, not the transform directly for pathing
    private bool isInitialized = false;
    private const float NAVMESH_SAMPLE_RADIUS = 3.0f; // Increased slightly, adjust if needed

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null || animator == null)
        {
            // Debug.LogError($"Missing NavMeshAgent or Animator on {gameObject.name}", this);
            enabled = false; // Critical failure
            return;
        }

        // Read component settings for info/warnings (Good!)
        agentUpdateRotation = agent.updateRotation;
        animatorApplyRootMotion = animator.applyRootMotion;
        // if (!agentUpdateRotation) { Debug.LogWarning($"NavMeshAgent on {gameObject.name} has 'Update Rotation' set to FALSE. Character might not turn.", this); }
        // if (animatorApplyRootMotion) { Debug.LogWarning($"Animator on {gameObject.name} has 'Apply Root Motion' set to TRUE. This can conflict with NavMeshAgent.", this); }
    }

    void Start()
    {
        // Initial state setup
        currentState = NPCState.Initializing;
        SetAnimatorMovement(false, false); // Visually idle
        agent.isStopped = true; // Stop movement until initialized
        isInitialized = false; // Ensure initialization runs
        // --- Start the initialization Coroutine ---
        StartCoroutine(InitializeAgentAndStart());
    }

    // Update is mainly for state machine *after* initialization
    void Update()
    {
        // Don't run state logic until fully initialized
        if (!isInitialized || currentState == NPCState.Initializing) return;

        // State machine logic
        switch (currentState)
        {
            case NPCState.RunningToInitialTarget:
                UpdateRunningToTargetState();
                break;
            case NPCState.Wandering_Deciding:
                UpdateWanderingDecidingState();
                break;
            case NPCState.Wandering_Moving:
                UpdateWanderingMovingState();
                break;
        }
    }

    IEnumerator InitializeAgentAndStart()
    {
        // Debug.Log($"{gameObject.name}: Starting Initialization Coroutine.");

        // 1. Essential: Check if the target was actually assigned by the spawner
        if (initialTargetDestination == null)
        {
            // Debug.LogError($"INITIAL TARGET DESTINATION IS NULL for {gameObject.name} after spawn! Spawner might not have assigned it or it was lost. Disabling AI.", this);
            currentState = NPCState.Initializing; // Stay in init (or go to an error state)
            SetAnimatorMovement(false, false);
            agent.isStopped = true;
            enabled = false; // Disable if target is missing
            yield break; // Stop coroutine
        }
        initialTargetPosition = initialTargetDestination.position; // Cache the position
        // Debug.Log($"{gameObject.name}: Target '{initialTargetDestination.name}' confirmed at {initialTargetPosition}.");


        // 2. Wait a frame to allow the object's position to stabilize in the physics/NavMesh world
        yield return new WaitForEndOfFrame();
        // Debug.Log($"{gameObject.name}: EndOfFrame wait complete. Current Position: {transform.position}");


        // 3. Ensure Agent is on the NavMesh - VERY IMPORTANT FOR SPAWNED OBJECTS
        if (!agent.isOnNavMesh)
        {
            // Debug.LogWarning($"{gameObject.name} is not on NavMesh at {transform.position}. Attempting to sample and warp...");
            NavMeshHit hit;
            // Try to find the nearest valid point within a radius
            if (NavMesh.SamplePosition(transform.position, out hit, NAVMESH_SAMPLE_RADIUS, NavMesh.AllAreas))
            {
                // Debug.Log($"{gameObject.name}: Found NavMesh point at {hit.position} (distance: {hit.distance}). Attempting Warp...");
                if (agent.Warp(hit.position))
                {
                    // Debug.Log($"{gameObject.name}: Warp successful to {agent.transform.position}. Waiting another frame..."); // Agent position updates AFTER warp
                     // Need another brief wait AFTER warp for the agent's internal state to fully update
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    // Warp can fail if the agent is not active or other issues occur
                    // Debug.LogError($"{gameObject.name}: Warp to {hit.position} FAILED! Agent might be inactive or invalid state. Disabling AI.", this);
                    currentState = NPCState.Initializing;
                    SetAnimatorMovement(false, false);
                    agent.isStopped = true;
                    enabled = false;
                    yield break;
                }
            }
            else
            {
                // Debug.LogError($"{gameObject.name}: SamplePosition failed - Could not find ANY valid NavMesh position within {NAVMESH_SAMPLE_RADIUS} units of {transform.position}! Check NavMesh bake and Spawner position. Disabling AI.", this);
                currentState = NPCState.Initializing;
                SetAnimatorMovement(false, false);
                agent.isStopped = true;
                enabled = false;
                yield break; // Stop if we can't get on the mesh
            }
        } else {
             // Debug.Log($"{gameObject.name}: Agent is already on NavMesh at {transform.position}.");
        }

        // 4. Final Check and Start Movement
        if (!agent.isOnNavMesh) {
            // Debug.LogError($"{gameObject.name}: Agent STILL not on NavMesh after warp attempt! Disabling AI.", this);
            currentState = NPCState.Initializing;
            SetAnimatorMovement(false, false);
            agent.isStopped = true;
            enabled = false;
            yield break;
        }

        // Debug.Log($"{gameObject.name}: Initialization Complete. Agent is on NavMesh. Proceeding to Run state.");
        isInitialized = true; // Mark initialization as complete *before* changing state
        GoToState(NPCState.RunningToInitialTarget); // Now safe to start the first state
    }

    void GoToState(NPCState nextState)
    {
        // Allow state change only if initialized OR if it's the final step of initialization
         if (!isInitialized && nextState != NPCState.RunningToInitialTarget)
         {
             // This might happen if an external script tries to change state too early
             // Debug.LogWarning($"Attempted state change to {nextState} on {gameObject.name} before initialization was complete. Ignoring.");
             return;
         }

        // Prevent redundant state changes
        // if (currentState == nextState) return; // Optional: uncomment if needed

        // --- Optional: Add Exit logic for the OLD state here if necessary ---
        // switch (currentState) { /* ... exit logic ... */ }

        // --- Set the new state ---
        currentState = nextState;
        // Debug.Log($"{gameObject.name}: Entering State: {currentState}"); // Add for debugging state changes

        // --- Entry logic for the NEW state ---
        switch (currentState)
        {
            case NPCState.RunningToInitialTarget:
                agent.speed = runningSpeed;
                agent.stoppingDistance = 0.1f; // Make sure it gets close
                 // Destination should ONLY be set when we are SURE the agent is valid and on the mesh
                if (agent.isOnNavMesh) // Final safety check
                {
                    if (agent.SetDestination(initialTargetPosition))
                    {
                         // Debug.Log($"{gameObject.name}: Setting destination to {initialTargetPosition}. Agent path pending: {agent.pathPending}");
                         agent.isStopped = false; // Make sure it can move
                         SetAnimatorMovement(true, false); // Running = true
                    } else {
                         // Debug.LogError($"{gameObject.name}: SetDestination FAILED even though agent is on NavMesh! Path might be invalid. Going back to Init.", this);
                         // Handle this failure - maybe the target position itself is invalid?
                         isInitialized = false; // Force re-initialization attempt
                         currentState = NPCState.Initializing;
                         SetAnimatorMovement(false, false);
                         agent.isStopped = true;
                         StartCoroutine(InitializeAgentAndStart()); // Retry initialization
                    }
                }
                else
                {
                    // This ideally shouldn't happen because of the Init coroutine, but is a safeguard
                    // Debug.LogError($"{gameObject.name}: Tried to run to target BUT agent is not on NavMesh! Critical error during state change. Going back to Init.", this);
                    isInitialized = false; // Force re-initialization attempt
                    currentState = NPCState.Initializing;
                    SetAnimatorMovement(false, false);
                    agent.isStopped = true;
                    StartCoroutine(InitializeAgentAndStart()); // Retry initialization
                }
                break;

            case NPCState.Wandering_Deciding:
                agent.isStopped = true; // Stop movement
                agent.ResetPath(); // Clear previous path
                SetAnimatorMovement(false, false); // Idle animation
                wanderWaitTimer = Random.Range(wanderMinWaitTime, wanderMaxWaitTime);
                // Debug.Log($"{gameObject.name}: Deciding wander... waiting for {wanderWaitTimer:F2}s.");
                break;

            case NPCState.Wandering_Moving:
                agent.speed = walkingSpeed;
                agent.stoppingDistance = 0.1f; // Use a small stopping distance
                Vector3 newWanderPoint = FindRandomPointInRadius(initialTargetPosition, wanderRadius); // Use cached initial target as center

                if (newWanderPoint != Vector3.zero && agent.isOnNavMesh)
                {
                     if(agent.SetDestination(newWanderPoint)) {
                        // Debug.Log($"{gameObject.name}: Wandering to {newWanderPoint}.");
                        agent.isStopped = false;
                        SetAnimatorMovement(false, true); // Walking = true
                     } else {
                        // Debug.LogWarning($"{gameObject.name}: Failed to set wander destination {newWanderPoint}. Re-deciding.", this);
                        GoToState(NPCState.Wandering_Deciding); // Try again
                     }
                }
                else
                {
                    // Failed to find a point or not on mesh
                    // Debug.LogWarning($"{gameObject.name}: Couldn't find wander point or not on NavMesh. Re-deciding.", this);
                    GoToState(NPCState.Wandering_Deciding); // Revert to deciding
                }
                break;
            // Add case for Idle/Error state if you implement one
        }
    }

    // --- State Update Methods ---
    void UpdateRunningToTargetState()
    {
        // Check if we've arrived
        // Use a small threshold for remaining distance check
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f) // Added tolerance
        {
            // Check if velocity is near zero to confirm arrival, not just close
            if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f)
            {
                // Debug.Log($"{gameObject.name}: Arrived at initial target.");
                GoToState(NPCState.Wandering_Deciding); // Transition to wandering
            }
        }
        // Optional: Add check for path becoming invalid?
        // else if (agent.pathStatus == NavMeshPathStatus.PathInvalid) { /* Handle error */ }
        else if (!agent.isStopped)
        {
            // Ensure animation stays correct while moving
            SetAnimatorMovement(true, false);
        }
    }

    void UpdateWanderingDecidingState()
    {
        wanderWaitTimer -= Time.deltaTime;
        if (wanderWaitTimer <= 0)
        {
            GoToState(NPCState.Wandering_Moving);
        }
        // Ensure visually idle
        SetAnimatorMovement(false, false);
    }

    void UpdateWanderingMovingState()
    {
        // Check for arrival at wander point
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f) // Added tolerance
        {
             if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f)
            {
                 // Debug.Log($"{gameObject.name}: Reached wander point.");
                GoToState(NPCState.Wandering_Deciding); // Go back to deciding
            }
        }
         // Optional: Add check for path becoming invalid?
        // else if (agent.pathStatus == NavMeshPathStatus.PathInvalid) { /* Handle error - maybe re-decide? */ GoToState(NPCState.Wandering_Deciding); }
        else if (!agent.isStopped)
        {
            // Ensure animation stays correct while moving
            SetAnimatorMovement(false, true);
        }
    }


    // --- Helper Functions ---
    Vector3 FindRandomPointInRadius(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++) // Limit attempts
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += center;
            NavMeshHit hit;
            // Sample for a valid position on the NavMesh
            if (NavMesh.SamplePosition(randomDirection, out hit, radius * 0.5f, NavMesh.AllAreas)) // Reduced sample distance slightly
            {
                return hit.position;
            }
        }
        // Debug.LogWarning($"Could not find valid wander point near {center} after 30 attempts.");
        return Vector3.zero; // Indicate failure
    }

    void SetAnimatorMovement(bool isRunning, bool isWalking)
    {
        if (animator == null) return;
        // Ensure walk is false if running
        animator.SetBool(runParamName, isRunning);
        animator.SetBool(walkParamName, isWalking && !isRunning);
    }

    // --- Gizmos ---
    void OnDrawGizmosSelected()
    {
        // Draw Wander Radius around INITIAL target
        if (initialTargetDestination != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(initialTargetDestination.position, wanderRadius);
        } else if (isInitialized) {
             // If target transform gone but we cached position, draw there
             Gizmos.color = Color.yellow;
             Gizmos.DrawWireSphere(initialTargetPosition, wanderRadius);
        }

        // Draw Agent Path
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.cyan;
            var path = agent.path;
            Vector3 prevCorner = transform.position;
            foreach (var corner in path.corners)
            {
                Gizmos.DrawLine(prevCorner, corner);
                Gizmos.DrawSphere(corner, 0.1f);
                prevCorner = corner;
            }
        }

        // Draw Current Destination
        if (agent != null && agent.hasPath) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(agent.destination, 0.2f);
        }
    }
}