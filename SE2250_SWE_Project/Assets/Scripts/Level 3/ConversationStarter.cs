using DialogueEditor;
using UnityEngine;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;

    private void OnTriggerStay(Collider other)
    {
        // Check if the object that entered the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Check if the 'E' key was pressed during this frame
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Try to get the PlayerInformation component from the player's GameObject
                PlayerInformation playerInfo = other.GetComponent<PlayerInformation>();
                

                // Check if the PlayerInformation component exists on the player object
                if (playerInfo != null)
                {
                    // Get the characterType value from the playerInfo component
                    int playerCharacterType = playerInfo.characterType;
                    bool hasPlayerTalked = playerInfo.HasTalked;
                    // You can access the public field directly
                    // Or use the property: int playerCharacterType = playerInfo.CharacterType;

                    // Start the conversation
                    ConversationManager.Instance.StartConversation(myConversation);

                    // Set the integer in the ConversationManager using the value from PlayerInformation
                    ConversationManager.Instance.SetInt("characterType", playerCharacterType);
                    ConversationManager.Instance.SetBool("hasTalked", hasPlayerTalked);

                    // Optional: Log for debugging to confirm the value
                    Debug.Log("Conversation started. Set characterType to: " + playerCharacterType);
                }
                else
                {
                    // Handle the case where the Player object doesn't have the PlayerInformation script
                    Debug.LogError("PlayerInformation component not found on the Player object!", other.gameObject);
                    // You might still want to start the conversation with a default value or show an error
                    // ConversationManager.Instance.StartConversation(myConversation);
                    // ConversationManager.Instance.SetInt("characterType", /* some default value, e.g., 0 or 7 */ );
                }
            }
        }
    }
}