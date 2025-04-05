using UnityEngine;
using UnityEngine.UI; // << ADD THIS! (Use TMPro if using TextMeshPro)
// using TMPro; // << OR ADD THIS if using TextMeshProUGUI
using System.Collections.Generic;
using System.Linq;
using TMPro;

[RequireComponent(typeof(SphereCollider))]
public class RecruiterDetection : MonoBehaviour
{
    [Header("Bot Detection")]
    [SerializeField] private string botTag = "Bot";
    [SerializeField] private float detectionRadius = 10.0f;

    [Header("UI Interaction (Optional)")]
    [Tooltip("Assign the instruction Text UI element here. It will be hidden after the first bot leaves the zone.")]
    // OR Use this line instead if you use TextMeshPro:
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("Status (Read Only)")]
    [SerializeField] private bool _isClearOfBotsReadOnly;
    [SerializeField] private int _initialBotCount; // Keep track of starting count

    // --- Internal State ---
    private List<Collider> botsInZone = new List<Collider>();
    private SphereCollider detectionSphere;
    private bool firstBotLeft = false; // Flag to track if the first bot has left

    // Public Property checks the LIST count
    public bool IsClearOfBots => botsInZone.Count <= 0;

    public void ReportBotLeaving(Collider botCollider)
    {
        if (botCollider != null && botsInZone.Contains(botCollider))
        {
            bool removed = botsInZone.Remove(botCollider);
            if (removed) // Only proceed if removal was successful
            {
                Debug.Log($"--> Bot {botCollider.name} reported leaving/destroyed. Removing from list. Count: {botsInZone.Count}");
                UpdateStatus(); // Update readonly status

                // --- NEW: Check if this is the first bot leaving ---
                if (!firstBotLeft)
                {
                    firstBotLeft = true; // Set the flag so this only runs once
                    HideInstructionText();
                }
                // ----------------------------------------------------
            }
        }
    }

    void Awake()
    {
        detectionSphere = GetComponent<SphereCollider>();
        if (detectionSphere != null)
        {
            detectionSphere.isTrigger = true;
            detectionSphere.radius = detectionRadius;
        }
        else
        {
            Debug.LogError("RecruiterDetection requires a SphereCollider component!", this);
            enabled = false;
            return;
        }

        // Ensure instruction text is visible initially (optional robustness)
        if (instructionText != null && !instructionText.gameObject.activeSelf)
        {
            // Only enable if there are potentially bots to kill
             CheckForInitialBots(); // Need count before deciding
             if (botsInZone.Count > 0)
             {
                  Debug.Log("Ensuring instruction text is visible at start.");
                  instructionText.gameObject.SetActive(true);
             }
        } else {
             CheckForInitialBots(); // Check normally if text is already visible or null
        }

        UpdateStatus();
    }

    void FixedUpdate()
    {
        // Remove any bots from the list that were destroyed unexpectedly
        int removedCount = botsInZone.RemoveAll(botCollider => botCollider == null || !botCollider.gameObject.activeInHierarchy);
        if (removedCount > 0)
        {
            Debug.LogWarning($"RecruiterDetection: Removed {removedCount} null or inactive bots from tracking list via FixedUpdate.");
            UpdateStatus();

            // --- NEW: Also check here if first bot left unexpectedly ---
            if (!firstBotLeft && botsInZone.Count < _initialBotCount) // Check against initial count
            {
                 firstBotLeft = true;
                 HideInstructionText();
                 Debug.Log("First bot removal detected via FixedUpdate cleanup.");
            }
            // ---------------------------------------------------------
        }
    }

    // --- Other methods (OnValidate, OnTriggerEnter, OnTriggerExit, LogBotStatus, OnDrawGizmosSelected) remain the same ---
     void OnValidate()
    {
        if (detectionSphere == null) detectionSphere = GetComponent<SphereCollider>();
        if (detectionSphere != null) detectionSphere.radius = detectionRadius;
         UpdateStatus();
    }

    void CheckForInitialBots()
    {
        botsInZone.Clear(); // Clear the list first
        Collider[] initialColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider col in initialColliders)
        {
            if (col.CompareTag(botTag) && col.gameObject != this.gameObject)
            {
                if (!botsInZone.Contains(col)) // Avoid duplicates if already added
                {
                     botsInZone.Add(col);
                }
            }
        }
        // Store initial count AFTER checking
        _initialBotCount = botsInZone.Count;
        LogBotStatus($"Initial Check. Bots found: {_initialBotCount}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(botTag))
        {
            if (!botsInZone.Contains(other)) // Add only if not already present
            {
                botsInZone.Add(other);
                LogBotStatus($"---> Bot Entered: {other.name}. Count: {botsInZone.Count}");
                 UpdateStatus();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(botTag))
        {
            ReportBotLeaving(other);
        }
    }

    void UpdateStatus()
    {
        _isClearOfBotsReadOnly = IsClearOfBots;
        // Note: We don't update _initialBotCount here, only in CheckForInitialBots
    }

    void LogBotStatus(string messagePrefix = "")
    {
         Debug.Log($"{messagePrefix} | {gameObject.name}: IsClearOfBots: {IsClearOfBots} (Current Count: {botsInZone.Count})");
    }

    void OnDrawGizmosSelected()
    {
        if (detectionSphere == null) return;
        Gizmos.color = IsClearOfBots ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    // ----------------------------------------------------------------------------------


    // --- NEW METHOD to hide the text ---
    private void HideInstructionText()
    {
        if (instructionText != null)
        {
            Debug.Log($"First bot left/destroyed. Hiding instruction text: {instructionText.name}");
            instructionText.gameObject.SetActive(false); // Disable the GameObject the Text component is on
        }
        else
        {
            Debug.LogWarning("Tried to hide instruction text, but no Text component was assigned in the Inspector!", this);
        }
    }
    // ---------------------------------
}