using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Image
using System.Linq;    // Required for checking if any bots were found

public class LevelCompletionManager : MonoBehaviour
{
    [Header("Setup References")]
    [SerializeField] private Canvas uiCanvas;             // Assign the Canvas containing the progress bar
    [SerializeField] private Image progressBarImage;      // Assign the 'Filled' Image for the progress bar
    [SerializeField] private RecruiterScript recruiter;  // Assign the GameObject with the RecruiterScript
    [SerializeField] private LayerMask botLayerMask;      // IMPORTANT: Set this in the Inspector to the layer your Bots are on
    // OR uncomment the line below and assign the tag if you prefer tag checking
    // [SerializeField] private string botTag = "Bot";

    [Header("Timer Settings")]
    [SerializeField] private float maxTime = 60f;         // Maximum time in seconds

    private float currentTime = 0f;
    private bool isTimerPaused = false;

    void Start()
    {
        // Initial setup
        if (uiCanvas != null)
        {
            // Ensure canvas is active from the start
            uiCanvas.gameObject.SetActive(true);
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

        // Initialize timer state
        currentTime = 0f;
        isTimerPaused = false;
        UpdateProgressBar(); // Update bar to show 0% initially
    }

    void Update()
    {
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
        //     // You might want to stop further updates here if the level ends
        //     enabled = false; // Disable this script component
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
            // Ensure we're not detecting the recruiter itself if it's on the same layer
            if(col.transform != recruiter.transform)
            {
                // Optional: Uncomment if using tag check instead of/in addition to LayerMask
                // if (col.CompareTag(botTag))
                // {
                    botFound = true;
                    break; // Found one bot, no need to check further
                // }
            }
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

    // --- Trigger Zone Detection Methods Removed ---
    // private void OnTriggerEnter(Collider other) { ... }
    // private void OnTriggerExit(Collider other) { ... }
}