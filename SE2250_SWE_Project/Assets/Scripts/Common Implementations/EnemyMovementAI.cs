using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class EnemyMovementAI : MonoBehaviour
{
    [Header("Detection & Idle Target")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform fixedDestination;

    [Header("Movement")]
    [SerializeField] protected float speed = 3.5f;
    [SerializeField] private float stoppingDistance = 1.5f;
    [SerializeField] private float rotationSpeed = 120f;

    private Transform currentTarget;
    private Transform playerTransform;
    private bool isPlayerDetected = false;
    private Vector3 initialPosition;

    void Awake()
    {
        initialPosition = transform.position;
    }

    protected virtual void Start()
    {
        // Try to find player immediately
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            Debug.Log($"'{name}' found player immediately: {playerTransform.name}", this);
        }

        // Start coroutine just in case player isn't spawned yet
        StartCoroutine(WaitForPlayer());

        // Set to idle if needed
        if (playerTransform == null && fixedDestination != null)
        {
            currentTarget = fixedDestination;
            Debug.Log($"'{name}' starting in idle mode. Target: {currentTarget.name}", this);
        }
        else if (fixedDestination == null)
        {
            Debug.LogWarning($"'{name}' has no fixed destination assigned. Will stand still if player is not found.", this);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        HandleDetectionAndTargetSwitch();
        PerformMovement();
    }

    IEnumerator WaitForPlayer()
    {
        while (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                Debug.Log($"'{name}' eventually found player: {playerTransform.name}", this);
                HandleDetectionAndTargetSwitch(); // immediately react
                yield break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void HandleDetectionAndTargetSwitch()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius)
        {
            if (!isPlayerDetected || currentTarget != playerTransform)
            {
                isPlayerDetected = true;
                currentTarget = playerTransform;
                Debug.Log($"'{name}' detected player. Switching to chase mode.", this);
            }
        }
        else
        {
            if (isPlayerDetected)
            {
                isPlayerDetected = false;
                currentTarget = fixedDestination;
                Debug.Log($"'{name}' lost player. Returning to idle mode.", this);
            }
        }
    }

    void PerformMovement()
    {
        if (currentTarget == null) return;

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance > stoppingDistance)
        {
            MoveTowardsTarget();
            RotateTowardsTarget();
        }
        else if (isPlayerDetected)
        {
            RotateTowardsTarget(); // Keeps looking at player while in range
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * speed * Time.deltaTime;
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void AssignFixedDestination(Transform destination)
    {
        fixedDestination = destination;
        if (!isPlayerDetected)
        {
            currentTarget = destination;
        }
    }

    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isPlayerDetected ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }

        if (fixedDestination != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(fixedDestination.position, 0.5f);
        }
    }
}
