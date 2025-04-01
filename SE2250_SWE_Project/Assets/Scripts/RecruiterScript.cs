using UnityEngine;
using System.Linq; // Still useful if you wanted to sort, but not strictly needed for the OverlapSphere approach

public class RecruiterScript : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 5f;       // The maximum distance to look for a target
    public LayerMask targetLayers;             // Assign the Layers your Players and Bots are on in the Inspector

    [Header("Rotation Settings")]
    [Tooltip("Lower values make the rotation slower and smoother.")]
    public float rotationSpeed = 2f;         // How quickly the recruiter turns. Lower value = smoother/slower.

    private Transform currentTarget = null;     // Cache the currently targeted object

    // Update is called once per frame
    void Update()
    {
        FindAndTargetNearest();
        LookAtTarget();
    }

    void FindAndTargetNearest()
    {
        // Find all colliders within the radius that are on the specified layers
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, targetLayers);

        Transform closestTarget = null;
        float minDistanceSqr = detectionRadius * detectionRadius; // Use squared distance

        // Iterate through all found colliders
        foreach (Collider hitCollider in hitColliders)
        {
            // --- Make sure we don't target ourselves! ---
            if (hitCollider.transform == this.transform)
            {
                continue; // Skip to the next collider
            }

            // Calculate the squared distance from the recruiter to the potential target
            float distanceSqr = (hitCollider.transform.position - transform.position).sqrMagnitude;

            // Check if this target is closer than the current closest
            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                closestTarget = hitCollider.transform;
            }
        }

        // Update the current target
        currentTarget = closestTarget;
    }

    void LookAtTarget()
    {
        // If there is a valid target
        if (currentTarget != null)
        {
            // Calculate the direction vector from the recruiter to the target
            Vector3 direction = currentTarget.position - transform.position;

            // --- IMPORTANT: Ignore vertical difference for smooth horizontal rotation ---
            direction.y = 0;

            // Ensure the direction is not zero (can happen if target is exactly at recruiter's position)
            // Check against a small threshold to avoid issues with LookRotation demanding a non-zero vector
            if (direction.sqrMagnitude > 0.001f)
            {
                // Calculate the rotation needed to look in that direction
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Smoothly rotate towards the target rotation over time
                // Lower rotationSpeed makes this smoother (takes longer)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        // Optional: Add an 'else' here if you want the recruiter to return
        // to a default facing direction when no target is in range.
        // else { /* Logic to return to default rotation */ }
    }

    // Optional: Draw a wire sphere in the Scene view to visualize the detection radius
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; // Changed color slightly
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Also draw a line to the current target if one exists
        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}