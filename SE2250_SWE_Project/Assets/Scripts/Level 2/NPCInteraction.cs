using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public TextMeshProUGUI interactionText;  // UI text for showing the interaction prompt
    public TextMeshProUGUI feedbackText;     // UI text for showing feedback after interaction
    public TextMeshProUGUI dialogueText;     // UI text for showing the NPC's dialogue
    public GameObject dialoguePanel;         // Background panel for the dialogue box
    public GameObject dialogueCanvas;        // The Canvas containing the dialogue UI
    public GameObject portalPrefab;          // Reference to the portal prefab
    public Transform portalSpawnPoint;       // The position where the portal will spawn

    private bool isPlayerNear = false;       // Flag to check if the player is close enough
    private GameObject player;               // Reference to the player object
    private bool portalSpawned = false;      // Flag to check if the portal has been spawned

    private void Start()
    {
        // Initially hide the dialogue UI
        dialogueCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if the player entered the NPC's collider
        {
            player = other.gameObject;
            isPlayerNear = true;  // Set flag to true when the player is nearby
            interactionText.text = "Press E to interact";  // Show interaction prompt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;  // Set flag to false when the player exits the collider
            feedbackText.text = "";     // Clear feedback text when player is not near
            dialogueCanvas.SetActive(false);  // Hide the dialogue UI when the player is not interacting
        }
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))  // Check if the player presses E
        {
            interactionText.text = "";  // Hide the prompt
            InteractWithNPC();
        }
    }

    private void InteractWithNPC()
    {
        // Get the ManageSkills component from the player
        ManageSkills skillManager = player.GetComponent<ManageSkills>();

        if (skillManager != null)
        {
            // Show the NPC's dialogue when interacting
            dialogueCanvas.SetActive(true);  // Show the dialogue canvas
            dialogueText.text = "NPC: Hello, little engineer!";  // NPC greeting

            // Check if the player has all required skills
            if (skillManager.HasAllRequiredSkills())
            {
                feedbackText.text = "You have all the required skills! Good luck!";  // Success message
                Debug.Log("Player has all required skills.");
                
                // Spawn the portal if it hasn't been spawned yet
                if (!portalSpawned)
                {
                    SpawnPortal();
                }
            }
            else
            {
                feedbackText.text = "Your resume isn't strong enough, you need more skills to proceed.";  // Failure message
                Debug.Log("Player is missing required skills.");
                // List missing skills or show a hint for the player
            }
        }
        else
        {
            Debug.LogError("No ManageSkills component found on the player.");
        }
    }

    // Function to spawn the portal
    private void SpawnPortal()
    {
        if (portalPrefab != null && portalSpawnPoint != null)
        {
            Instantiate(portalPrefab, portalSpawnPoint.position, portalSpawnPoint.rotation);
            portalSpawned = true;  // Mark the portal as spawned
        }
        else
        {
            Debug.LogError("PortalPrefab or PortalSpawnPoint is not assigned!");
        }
    }
}




