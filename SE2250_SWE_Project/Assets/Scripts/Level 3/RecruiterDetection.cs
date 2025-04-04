// RecruiterDetection.cs
using UnityEngine;
using System.Collections.Generic; // Required for List
using System.Linq; // Required for removing nulls

[RequireComponent(typeof(SphereCollider))]
public class RecruiterDetection : MonoBehaviour
{
    [Header("Bot Detection")]
    [SerializeField] private string botTag = "Bot";
    [SerializeField] private float detectionRadius = 10.0f;

    [Header("Status (Read Only)")]
    [SerializeField]
    private bool _isClearOfBotsReadOnly;

    // --- Store the actual colliders currently inside ---
    private List<Collider> botsInZone = new List<Collider>();

    private SphereCollider detectionSphere;

    // --- Public Property checks the LIST count ---
    public bool IsClearOfBots => botsInZone.Count <= 0;

    // --- Public method for bots to report they are being destroyed ---
    // Call this EXTERNALLY before destroying a bot
    public void ReportBotLeaving(Collider botCollider)
    {
        if (botCollider != null && botsInZone.Contains(botCollider))
        {
            botsInZone.Remove(botCollider);
            Debug.Log($"--> Bot {botCollider.name} reported leaving/destroyed. Removing from list. Count: {botsInZone.Count}");
            UpdateStatus(); // Update readonly status
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

        CheckForInitialBots(); // Clear list and check at start
        UpdateStatus();
    }

     // --- Optional: Add a periodic check for robustness ---
    void FixedUpdate() // Use FixedUpdate for physics-related checks
    {
        // Remove any bots from the list that were destroyed unexpectedly
        // (e.g., if ReportBotLeaving wasn't called correctly)
        int removedCount = botsInZone.RemoveAll(botCollider => botCollider == null || !botCollider.gameObject.activeInHierarchy);
        if (removedCount > 0)
        {
             Debug.LogWarning($"RecruiterDetection: Removed {removedCount} null or inactive bots from tracking list.");
             UpdateStatus();
        }
    }


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
        LogBotStatus($"Initial Check. Bots found: {botsInZone.Count}");
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"TRIGGER ENTER detected: {other.name} with tag '{other.tag}'");
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
        // Debug.Log($"TRIGGER EXIT detected: {other.name} with tag '{other.tag}'");
        if (other.CompareTag(botTag))
        {
            // Use the public method to handle removal consistently
            ReportBotLeaving(other);
            // Note: UpdateStatus() is called within ReportBotLeaving
            // LogBotStatus($"---> Bot Exited: {other.name}. Count: {botsInZone.Count}"); // Log already in ReportBotLeaving
        }
    }

    void UpdateStatus()
    {
        _isClearOfBotsReadOnly = IsClearOfBots;
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
}