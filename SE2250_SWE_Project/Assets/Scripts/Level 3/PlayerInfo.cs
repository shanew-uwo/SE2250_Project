using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public bool hasTalkedToSpecificNPC = false;
    public int characterType = 0; // Assuming this is used elsewhere

    // Called by the Event Node in the first NPC's dialogue
    public void MarkAsTalkedToSpecificNPC()
    {
        // --- DEBUG ---
        Debug.Log($"===== {this.gameObject.name} ({Time.frameCount}): MarkAsTalkedToSpecificNPC() called. Setting hasTalkedToSpecificNPC = true. =====");
        hasTalkedToSpecificNPC = true;
        // Optional: Add saving logic here if needed
    }

    // Called by ConditionalDialogueTrigger to check the status
    public bool CheckIfTalkedToSpecificNPC()
    {
        // --- DEBUG ---
        Debug.Log($"--- {this.gameObject.name} ({Time.frameCount}): CheckIfTalkedToSpecificNPC() called. Returning: {hasTalkedToSpecificNPC} ---");
        return hasTalkedToSpecificNPC;
    }

    public int checkCharacterType() // Assuming this is used elsewhere
    {
        return characterType;
    }
}