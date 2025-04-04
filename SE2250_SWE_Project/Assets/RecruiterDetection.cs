using UnityEngine;

// Ensure the GameObject has a SphereCollider component
[RequireComponent(typeof(SphereCollider))]
public class RecruiterBotDetector : MonoBehaviour
{
    [Tooltip("The tag assigned to the bot GameObjects.")]
    [SerializeField] private string botTag = "Bot"; // Make sure your bots have this exact tag

    [Tooltip("The radius around the recruiter to check for bots.")]
    [SerializeField] private float detectionRadius = 10.0f; // Adjust as needed

    // --- Private variable to track bots currently inside ---
    private int numberOfBotsInZone = 0;

    // --- Public Property to check the state ---
    // Returns TRUE if NO bots are in the zone, FALSE otherwise.
    public bool IsClearOfBots
    {
        get { return numberOfBotsInZone <= 0; }
    }

    private SphereCollider detectionSphere;

    void Awake()
    {
        // Get the SphereCollider component
        detectionSphere = GetComponent<SphereCollider>();

        // --- Configure the SphereCollider ---
        if (detectionSphere != null)
        {
            detectionSphere.isTrigger = true; // Ensure it acts as a trigger
            detectionSphere.radius = detectionRadius; // Set its size based on our variable
        }
        else
        {
            Debug.LogError("RecruiterBotDetector requires a SphereCollider component attached!", this);
            enabled = false; // Disable the script if no collider is found
        }

        // Optional: Add a Rigidbody if trigger events aren't firing reliably
        // Ensure Rigidbody is present and set to kinematic if you don't want physics affecting the recruiter
        /*
        if (GetComponent<Rigidbody>() == null) {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        */

        // Initial check in case bots start inside the radius
        CheckForInitialBots();
    }

    // Optional: Update radius in editor if value changes
    void OnValidate()
    {
        if (detectionSphere == null)
        {
            detectionSphere = GetComponent<SphereCollider>();
        }
        if (detectionSphere != null)
        {
            detectionSphere.radius = detectionRadius;
        }
    }

    // Check for bots already inside the trigger when the game starts
    void CheckForInitialBots()
    {
        numberOfBotsInZone = 0; // Reset count
        Collider[] initialColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider col in initialColliders)
        {
            // Check the tag and make sure it's not the recruiter itself
            if (col.CompareTag(botTag) && col.gameObject != this.gameObject)
            {
                numberOfBotsInZone++;
            }
        }
        // Log initial state
        LogBotStatus();
    }


    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering has the correct tag
        if (other.CompareTag(botTag))
        {
            numberOfBotsInZone++;
            LogBotStatus(); // Log the change
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the object exiting has the correct tag
        if (other.CompareTag(botTag))
        {
            numberOfBotsInZone--;
            // Prevent count from going negative due to any physics quirks
            if (numberOfBotsInZone < 0)
            {
                numberOfBotsInZone = 0;
            }
            LogBotStatus(); // Log the change
        }
    }

    // Helper function for debugging
    void LogBotStatus()
    {
         if (IsClearOfBots)
         {
              Debug.Log($"{gameObject.name}: Zone is clear of bots. (Count: {numberOfBotsInZone})");
         }
         else
         {
              Debug.Log($"{gameObject.name}: Bots DETECTED in zone! (Count: {numberOfBotsInZone})");
         }
         // You can directly check the property here: Debug.Log("IsClearOfBots: " + IsClearOfBots);
    }

    // Optional: Visualize the detection radius in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}