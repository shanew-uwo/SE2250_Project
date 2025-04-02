using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent
using System.Collections; // Required for Coroutines

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCMovementAI : MonoBehaviour
{
    // Keep your existing enum and public variables
    public enum NPCState
    {
        Initializing, // NEW: State for waiting setup
        RunningToInitialTarget,
        Wandering_Deciding,
        Wandering_Moving
    }

    [Header("Movement Target")]
    public Transform initialTargetDestination;

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
    [SerializeField] private bool agentUpdateRotation = true;
    [SerializeField] private bool animatorApplyRootMotion = false;

    private NavMeshAgent agent;
    private Animator animator;
    private NPCState currentState;
    private float wanderWaitTimer;
    private Vector3 initialTargetPosition;
    private bool isInitialized = false; // Flag to prevent multiple initializations


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Basic component checks
        if (agent == null || animator == null)
        {
            Debug.LogError($"Missing NavMeshAgent or Animator on {gameObject.name}", this);
            enabled = false;
            return;
        }
         // Verification checks (keep these)
        if (!agent.updateRotation) { Debug.LogWarning($"NavMeshAgent on {gameObject.name} has 'Update Rotation' set to FALSE.", this); agentUpdateRotation = false; }
        if (animator.applyRootMotion) { Debug.LogWarning($"Animator on {gameObject.name} has 'Apply Root Motion' set to TRUE. This can conflict with NavMeshAgent.", this); animatorApplyRootMotion = true; }
    }

    void Start()
    {
        // Don't do the main logic here directly anymore
        // Set initial state
        currentState = NPCState.Initializing;
        SetAnimatorMovement(false, false); // Start visually idle
        agent.isStopped = true; // Ensure it's not moving initially
    }

    void Update()
    {
        // Initialization Check - Run only once
        if (!isInitialized && currentState == NPCState.Initializing)
        {
            StartCoroutine(InitializeAgentAndStart());
            isInitialized = true; // Prevent this from running again
            return; // Skip rest of update during initialization frame
        }

        // Only run state updates after initialization is complete
        if (currentState == NPCState.Initializing) return;


        // Update logic based on the current state (keep your existing switch)
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
        // 1. Check Target
        if (initialTargetDestination == null)
        {
            Debug.LogError($"Initial Target Destination is not set for {gameObject.name}", this);
            enabled = false; // Disable script if no target
            yield break; // Stop the coroutine
        }
        initialTargetPosition = initialTargetDestination.position;


        // 2. Wait for NavMeshAgent to be valid (crucial for spawned objects)
        // Wait for the end of the frame to allow physics/NavMesh system to update
        yield return new WaitForEndOfFrame();

        // Try to warp to nearest NavMesh position if not already on it
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"{gameObject.name} was not spawned directly on NavMesh. Attempting to warp...");
            NavMeshHit hit;
            // Search within 2 units for a valid spot
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                if(agent.Warp(hit.position)) {
                     Debug.Log($"{gameObject.name} warped successfully to {hit.position}.");
                     // Need another brief wait after warp for agent state to settle
                     yield return new WaitForEndOfFrame();
                } else {
                     Debug.LogError($"{gameObject.name} failed to warp to NavMesh near {hit.position}! Disabling AI.", this);
                     enabled = false;
                     yield break;
                }
            }
            else
            {
                Debug.LogError($"{gameObject.name} could not find valid NavMesh position near {transform.position} to warp to! Disabling AI.", this);
                enabled = false;
                yield break; // Stop if we can't get on the mesh
            }
        }

        // 3. Now that we are reasonably sure the agent is on the mesh, start moving
        Debug.Log($"{gameObject.name} Initialized. Starting movement.");
        GoToState(NPCState.RunningToInitialTarget);
    }


    // --- State Management (GoToState needs slight adjustment) ---

    void GoToState(NPCState nextState)
    {
        // Only proceed if initialized (prevent calls during initial setup)
        if (currentState == NPCState.Initializing && nextState != NPCState.RunningToInitialTarget && !isInitialized)
        {
            Debug.LogWarning($"Attempted to change state to {nextState} before initialization completed.");
            return;
        }

        currentState = nextState;

        // Entry logic for new state (Your existing logic here is mostly fine)
        switch (currentState)
        {
            case NPCState.RunningToInitialTarget:
                agent.speed = runningSpeed;
                agent.isStopped = false; // Ensure agent can move
                // Set Destination AFTER ensuring agent is valid (handled by InitializeAgentAndStart)
                if (agent.isOnNavMesh) // Double check before setting destination
                {
                    agent.SetDestination(initialTargetPosition);
                    SetAnimatorMovement(true, false); // Running = true, Walking = false
                } else {
                    Debug.LogError($"{gameObject.name} is not on NavMesh when trying to run to target!", this);
                     // Handle error - maybe try re-initializing or go idle
                     isInitialized = false; // Signal to retry initialization
                     currentState = NPCState.Initializing;
                     SetAnimatorMovement(false, false);
                }
                break;
            // --- Keep your other Wandering state logic ---
            case NPCState.Wandering_Deciding:
                agent.isStopped = true;
                SetAnimatorMovement(false, false);
                wanderWaitTimer = Random.Range(wanderMinWaitTime, wanderMaxWaitTime);
                break;

            case NPCState.Wandering_Moving:
                agent.speed = walkingSpeed;
                Vector3 newWanderPoint = FindRandomPointInRadius(initialTargetPosition, wanderRadius);
                if (newWanderPoint != Vector3.zero && agent.isOnNavMesh)
                {
                    agent.SetDestination(newWanderPoint);
                    agent.isStopped = false;
                    SetAnimatorMovement(false, true);
                }
                else
                {
                    Debug.LogWarning($"{gameObject.name} couldn't find wander point or not on NavMesh, waiting again.");
                    GoToState(NPCState.Wandering_Deciding); // Revert to deciding
                }
                break;
        }
    }

    // --- Keep the rest of your State Update Logic, Helper Functions, and Gizmos ---
    // UpdateRunningToTargetState()
    // UpdateWanderingDecidingState()
    // UpdateWanderingMovingState()
    // FindRandomPointInRadius()
    // SetAnimatorMovement()
    // OnDrawGizmosSelected()

    // Make sure these methods exist from your previous script version:
    void UpdateRunningToTargetState()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
            {
                GoToState(NPCState.Wandering_Deciding);
            }
        }
        else if (!agent.isStopped)
        {
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
        SetAnimatorMovement(false, false);
    }

    void UpdateWanderingMovingState()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
            {
                GoToState(NPCState.Wandering_Deciding);
            }
        }
        else if (!agent.isStopped)
        {
            SetAnimatorMovement(false, true);
        }
    }
    Vector3 FindRandomPointInRadius(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++) { Vector3 randomDirection = Random.insideUnitSphere * radius; randomDirection += center; NavMeshHit hit; if (NavMesh.SamplePosition(randomDirection, out hit, radius * 1.5f, NavMesh.AllAreas)) { return hit.position; } }
        return Vector3.zero;
    }
    void SetAnimatorMovement(bool isRunning, bool isWalking)
    {
        if (animator == null) return; // Safety check
        animator.SetBool(runParamName, isRunning);
        animator.SetBool(walkParamName, isWalking && !isRunning);
    }
     void OnDrawGizmosSelected()
     {
         // Keep your gizmos code
          if (initialTargetDestination != null) { Gizmos.color = Color.green; Gizmos.DrawWireSphere(initialTargetDestination.position, wanderRadius); }
         if(agent != null && agent.hasPath) { Gizmos.color = Color.cyan; var path = agent.path; Vector3 prevCorner = transform.position; foreach(var corner in path.corners) { Gizmos.DrawLine(prevCorner, corner); Gizmos.DrawSphere(corner, 0.1f); prevCorner = corner; } }
     }
}