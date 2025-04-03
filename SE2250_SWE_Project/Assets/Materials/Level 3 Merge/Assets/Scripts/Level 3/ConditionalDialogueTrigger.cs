using UnityEngine;
using DialogueEditor; // Keep this if ConversationStarter is in this namespace

// Attach this script to the GameObject that has the SECOND ConversationStarter script (e.g., the Guard GameObject)
public class ConditionalDialogueTrigger : MonoBehaviour
{
    [Tooltip("Drag your Player GameObject (which has the PlayerInfo script) here.")]
    [SerializeField] private PlayerInfo _playerInfo;

    [Tooltip("Drag the ConversationStarter component (ON THIS SAME GameObject) that should be enabled/disabled.")]
    [SerializeField] private ConversationStarter _conversationStarterToControl;

    // --- DEBUG --- Added variable to prevent spamming logs if state is stable
    private bool? _lastReportedState = null;

    void Start()
    {
        // --- DEBUG --- Log the initial object references
        Debug.Log($"[{this.gameObject.name}] Start: Initializing ConditionalDialogueTrigger.");

        // Check PlayerInfo reference
        if (_playerInfo == null)
        {
            Debug.LogWarning($"[{this.gameObject.name}] Start: PlayerInfo not assigned in Inspector. Trying FindObjectOfType<PlayerInfo>()...");
            _playerInfo = FindObjectOfType<PlayerInfo>();
            if (_playerInfo == null) {
                 Debug.LogError($"[{this.gameObject.name}] Start ERROR: Failed to find PlayerInfo script in the scene! Assign it manually to {this.gameObject.name}. Conditional logic WILL FAIL.");
            } else {
                 Debug.Log($"[{this.gameObject.name}] Start: Found PlayerInfo on GameObject '{_playerInfo.gameObject.name}'.");
            }
        } else {
             Debug.Log($"[{this.gameObject.name}] Start: PlayerInfo assigned via Inspector to '{_playerInfo.gameObject.name}'.");
        }

        // Check ConversationStarter reference
        if (_conversationStarterToControl == null)
        {
            Debug.LogWarning($"[{this.gameObject.name}] Start: ConversationStarter to control not assigned in Inspector. Trying GetComponent<ConversationStarter>()...");
            _conversationStarterToControl = GetComponent<ConversationStarter>();
             if (_conversationStarterToControl == null) {
                 Debug.LogError($"[{this.gameObject.name}] Start ERROR: Failed to find ConversationStarter component on {this.gameObject.name}! Ensure it exists and assign it manually. Conditional logic WILL FAIL.");
            } else {
                Debug.Log($"[{this.gameObject.name}] Start: Found ConversationStarter on this GameObject.");
            }
        } else {
             Debug.Log($"[{this.gameObject.name}] Start: ConversationStarter assigned via Inspector.");
        }

        // Initial state check
        Debug.Log($"[{this.gameObject.name}] Start: Performing initial trigger state check.");
        UpdateTriggerState();
    }

    // Check every frame
    void Update()
    {
         UpdateTriggerState(); // Re-evaluate state every frame
    }

    void UpdateTriggerState()
    {
        // Check if references are valid FIRST
        if (_playerInfo == null || _conversationStarterToControl == null)
        {
            // --- DEBUG --- Log if references are missing (should have been caught in Start, but good safety check)
            // Add a check to prevent spamming this specific error every frame
            if (_lastReportedState != false) // Use _lastReportedState to track if we already logged the failure
            {
                 Debug.LogError($"[{this.gameObject.name}] UpdateTriggerState ({Time.frameCount}): Skipping update because PlayerInfo ({_playerInfo == null}) or ConversationStarter ({_conversationStarterToControl == null}) is null. Ensuring target is disabled.");
                 if (_conversationStarterToControl != null) _conversationStarterToControl.enabled = false; // Ensure it's off if refs are bad
                 _lastReportedState = false; // Mark that we logged the failure state
            }
            return; // Stop execution here if refs are invalid
        }

        // --- Core Logic ---
        // 1. Get the desired state based on player info
        bool shouldBeEnabled = _playerInfo.CheckIfTalkedToSpecificNPC(); // This line will now trigger the Debug.Log in PlayerInfo

        // 2. Get the current state of the target component
        bool currentIsEnabled = _conversationStarterToControl.enabled;

        // --- DEBUG --- Log current and desired states ONLY if they potentially mismatch the last known state to reduce spam
        if (_lastReportedState == null || currentIsEnabled != _lastReportedState.Value || shouldBeEnabled != _lastReportedState.Value)
        {
             Debug.Log($"[{this.gameObject.name}] UpdateTriggerState ({Time.frameCount}): Checking state... PlayerFlag (shouldBeEnabled) = {shouldBeEnabled}, Current Starter Enabled = {currentIsEnabled}");
        }


        // 3. Compare and apply the change IF NEEDED
        if (currentIsEnabled != shouldBeEnabled)
        {
            // --- DEBUG --- Log that we are changing the state
            Debug.Log($"<color=yellow>[{this.gameObject.name}] UpdateTriggerState ({Time.frameCount}): STATE MISMATCH! Setting ConversationStarter enabled state to: {shouldBeEnabled}</color>");

            _conversationStarterToControl.enabled = shouldBeEnabled;
            _lastReportedState = shouldBeEnabled; // Update last reported state after change
        }
        else
        {
             // --- DEBUG --- Log that no change is needed (only if state hasn't been logged as stable)
             if (_lastReportedState == null || currentIsEnabled != _lastReportedState.Value ) {
                 Debug.Log($"[{this.gameObject.name}] UpdateTriggerState ({Time.frameCount}): State matches (Current={currentIsEnabled}, Desired={shouldBeEnabled}). No change needed.");
             }
             _lastReportedState = currentIsEnabled; // Update last reported state even if no change occurred
        }
    }

    // Example function remains unchanged
    public void TryOtherInteraction()
    {
       if (_playerInfo != null && _playerInfo.CheckIfTalkedToSpecificNPC())
       {
           Debug.Log($"[{this.gameObject.name}] TryOtherInteraction: Allowed because player flag is true.");
       }
       else
       {
           Debug.Log($"[{this.gameObject.name}] TryOtherInteraction: Denied. Player Info: {(_playerInfo != null)}, Flag: {(_playerInfo != null ? _playerInfo.CheckIfTalkedToSpecificNPC().ToString() : "N/A")}");
       }
    }
}