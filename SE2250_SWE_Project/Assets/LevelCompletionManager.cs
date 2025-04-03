using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Image
using System.Linq;    // Required for checking if any bots were found

public class LevelCompletionManager : MonoBehaviour
{
    [Header("Setup References")]
    [SerializeField] private string playerTag = "Player"; // Tag of your player GameObject
    [SerializeField] private Canvas uiCanvas;             // Assign the Canvas containing the progress bar
    [SerializeField] private Image progressBarImage;      // Assign the 'Filled' Image for the progress bar
    [SerializeField] private RecruiterScript recruiter;  // Assign the GameObject with the RecruiterScript
    [SerializeField] private Collider triggerZone;       // Assign the Collider component of your trigger zone

    [Header("Timer Settings")]
    [SerializeField] private float maxTime = 60f;         // Maximum time in seconds

    [Header("Bot Detection")]
    [SerializeField] private LayerMask botLayerMask;      // IMPORTANT: Set this in the Inspector to the layer your Bots are on
    // OR uncomment the line below and assign the tag if you prefer tag checking
    // [SerializeField] private string botTag = "Bot";

    private float currentTime = 0f;
    private bool isLevelActive = false;
    private bool isTimerPaused = false;

    void Start()
    {
        // Initial setup
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(false); // Hide UI initially
        }
        else
        {
            Debug.LogError("LevelCompletionManager: UI Canvas not assigned!");
        }

        if (progressBarImage != null)
        {
            progressBarImage.fillAmount = 0f; // Reset progress bar
            if (progressBarImage.type != Image.Type.Filled || progressBarImage.fillMethod != Image.FillMethod.Horizontal)
            {
                Debug.LogWarning("LevelCompletionManager: Progress Bar Image should be Type=Filled and FillMethod=Horizontal.");
            }
        }
        else
        {
            Debug.LogError("LevelCompletionManager: Progress Bar Image not assigned!");
        }

        if (recruiter == null)
        {
            Debug.LogError("LevelCompletionManager: Recruiter Script not assigned!");
        }
        if (triggerZone == null)
        {
             Debug.LogError("LevelCompletionManager: Trigger Zone Collider not assigned! Trying to get it from this GameObject.");
             triggerZone = GetComponent<Collider>();
             if(triggerZone == null || !triggerZone.isTrigger)
             {
                 Debug.LogError("LevelCompletionManager: Could not find a trigger Collider on this GameObject.");
             }
        }
        else if (!triggerZone.isTrigger)
        {
             Debug.LogWarning("LevelCompletionManager: Assigned Trigger Zone Collider is not set to 'Is Trigger'.");
        }

        isLevelActive = false;
        currentTime = 0f;
    }

    void Update()
    {
        // Only run logic if the level area is active
        if (!isLevelActive) return;

        // Check if the timer should be paused based on bots near the recruiter
        CheckForBotsNearRecruiter();

        // Update the timer if not paused and not complete
        if (!isTimerPaused && currentTime < maxTime)
        {
            currentTime += Time.deltaTime;
            currentTime = Mathf.Min(currentTime, maxTime); // Clamp time to maxTime
        }

        // Update the progress bar UI
        UpdateProgressBar();

        // --- Optional: Add completion logic ---
        // if (currentTime >= maxTime)
        // {
        //     Debug.Log("Level Completed!");
        //     // Add actions here: show success message, load next level, etc.
        //     isLevelActive = false; // Stop processing after completion
        // }
    }

    void CheckForBotsNearRecruiter()
    {
        if (recruiter == null)
        {
            isTimerPaused = false; // Cannot check without a recruiter
            return;
        }

        // Find all colliders within the recruiter's radius on the specified bot layer
        Collider[] botsInRange = Physics.OverlapSphere(
            recruiter.transform.position,
            recruiter.detectionRadius,
            botLayerMask // Use the LayerMask for efficient filtering
        );

        // --- Check if ANY of the found colliders belong to a bot ---
        bool botFound = false;
        foreach (Collider col in botsInRange)
        {
            // You might not even need the tag check if the LayerMask is set correctly
            // if (col.CompareTag(botTag)) // Uncomment if using tag check instead of/in addition to LayerMask
            // {
                 // Make sure we're not detecting the recruiter itself if it's on the same layer
                if(col.transform != recruiter.transform)
                {
                    botFound = true;
                    break; // Found one bot, no need to check further
                }
            // }
        }


        isTimerPaused = botFound;

        // Optional: Visual feedback or debugging
        // Debug.Log($"Timer Paused: {isTimerPaused}");
    }

    void UpdateProgressBar()
    {
        if (progressBarImage != null)
        {
            float fillRatio = currentTime / maxTime;
            progressBarImage.fillAmount = fillRatio;
        }
    }

    // --- Trigger Zone Detection ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player entered level zone.");
            isLevelActive = true;
            currentTime = 0f; // Reset timer on entry
            isTimerPaused = false; // Reset pause state
            if (uiCanvas != null)
            {
                uiCanvas.gameObject.SetActive(true); // Show UI
            }
            UpdateProgressBar(); // Update bar to show 0%
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player exited level zone.");
            isLevelActive = false;
            if (uiCanvas != null)
            {
                uiCanvas.gameObject.SetActive(false); // Hide UI
            }
        }
    }
}