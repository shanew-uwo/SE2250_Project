using UnityEngine;
using UnityEngine.UI; // Need this for Image

public class PlayerBehaviorInjector : MonoBehaviour
{
    [Header("Player Component Settings")]
    [Tooltip("The tag assigned to player GameObjects.")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Max health to set on the player.")]
    [SerializeField] private float playerMaxHealth = 100f;


    [Header("AOE Attack Settings")]
    [Tooltip("Radius of the player's AOE attack.")]
    [SerializeField] private float playerAoeRadius = 5f;
    [Tooltip("Damage dealt by the player's AOE attack.")]
    [SerializeField] private float playerAoeDamage = 25f;
    [Tooltip("Cooldown time in seconds for the player's AOE attack.")]
    [SerializeField] private float playerAoeCooldown = 0.7f;
    // *** THIS is the array for multiple target tags ***
    [Tooltip("Tags the Player AOE Attack should damage (e.g., Bot, Enemy, Boss).")]
    [SerializeField] private string[] playerAoeTargetTags = { "Bot", "Enemy", "Boss" }; // Default values
    [Tooltip("Key to trigger the player's AOE attack.")]
    [SerializeField] private KeyCode playerAoeKey = KeyCode.Q;


    [Header("AOE Visuals")]
    [Tooltip("Assign Gradient asset for skill/color 1.")]
    public Gradient greenGradient;
    [Tooltip("Assign Gradient asset for skill/color 2.")]
    public Gradient orangeGradient;
    [Tooltip("Assign Gradient asset for skill/color 3.")]
    public Gradient purpleGradient;
    [Tooltip("Assign Gradient asset for skill/color 4.")]
    public Gradient blueGradient;
    [Tooltip("Assign the Material for the AOE LineRenderer.")]
    public Material lineMaterial;


    [Header("UI References")]
    [Tooltip("Name of the Canvas GameObject in the scene.")]
    [SerializeField] private string canvasName = "Canvas";
    [Tooltip("Name of the Damage Overlay Image GameObject under the Canvas.")]
    [SerializeField] private string damageOverlayName = "DamageOverlay";


    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);

        if (players.Length == 0)
        {
            Debug.LogError($"PlayerBehaviorInjector: No GameObjects found with tag '{playerTag}'!", this);
            return;
        }

        // Find UI elements once
        Image damageOverlayImage = FindDamageOverlay();

        foreach (GameObject player in players)
        {
            Debug.Log($"PlayerBehaviorInjector: Configuring player '{player.name}'...");

            // --- Remove Old Script (Optional safety check) ---
            // Ensure any potentially conflicting older interaction scripts are removed.
            // If 'KillingBots' script doesn't exist in your project anymore, you can remove this block.
            var oldInteraction = player.GetComponent("KillingBots"); // Use string version if class might not compile
            if (oldInteraction != null)
            {
                Debug.Log($"Removing potentially conflicting interaction script from {player.name}.");
                Destroy(oldInteraction);
            }

            // --- Ensure Health Component ---
            // Check if a Health component (or one inheriting from it, like PlayerHealth) exists.
            Health health = player.GetComponent<Health>();
            if (health == null)
            {
                // Add the appropriate health component.
                // If you have a specific PlayerHealth script, prefer adding that.
                // Otherwise, add the base Health script.
                #if PLAYER_HEALTH_SCRIPT_EXISTS // Example using custom scripting define symbol
                     health = player.AddComponent<PlayerHealth>();
                     Debug.Log($"Added PlayerHealth component to {player.name}.");
                #else
                     health = player.AddComponent<Health>();
                     Debug.Log($"Added base Health component to {player.name}.");
                #endif
            }
            else
            {
                Debug.Log($"{player.name} already has a {health.GetType().Name} component. Configuring existing.", health);
            }
            // Configure health properties
            health.SetMaxHealth(playerMaxHealth, true); // Set max health
            health.damageOverlay = damageOverlayImage; // Assign UI overlay


            // --- Ensure PlayerAOEAttack Component ---
            PlayerAOEAttack attack = player.GetComponent<PlayerAOEAttack>();
            if (attack == null)
            {
                attack = player.AddComponent<PlayerAOEAttack>();
                Debug.Log($"Added PlayerAOEAttack component to {player.name}.");
            }

            // Configure AOE Attack settings from Inspector fields
            attack.aoeRadius = playerAoeRadius;
            attack.aoeDamage = playerAoeDamage;
            attack.cooldown = playerAoeCooldown;
            // attack.attackKey = playerAoeKey; // This is set via [SerializeField] in PlayerAOEAttack now


            // *** Assign the ARRAY of target tags ***
            attack.targetTags = playerAoeTargetTags;
            // Log the assigned tags for verification
            if (playerAoeTargetTags != null && playerAoeTargetTags.Length > 0) {
                Debug.Log($"Set PlayerAOEAttack targetTags to: [{string.Join(", ", playerAoeTargetTags)}]");
            } else {
                 Debug.LogWarning($"PlayerAOEAttack targetTags set to empty or null by Injector.");
            }


            // --- Assign AOE Color Gradients ---
            // Ensure the aoeColors array exists in the PlayerAOEAttack script instance
            if (attack.aoeColors == null || attack.aoeColors.Length != 4) {
                 attack.aoeColors = new Gradient[4]; // Initialize/Resize if needed
            }
            // Assign gradients from Inspector fields
            attack.aoeColors[0] = greenGradient;
            attack.aoeColors[1] = orangeGradient;
            attack.aoeColors[2] = purpleGradient;
            attack.aoeColors[3] = blueGradient;


            // --- Ensure and Configure LineRenderer ---
            LineRenderer lr = player.GetComponent<LineRenderer>();
            if (lr == null)
            {
                lr = player.AddComponent<LineRenderer>();
                Debug.Log($"Added LineRenderer component to {player.name}.");
            }

            // Configure LineRenderer properties
            lr.useWorldSpace = true;
            lr.loop = true; // Make it a closed circle
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material = lineMaterial; // Assign the material from Inspector
            lr.positionCount = 0; // Start with 0 points until AOE is used
            lr.enabled = false; // Start disabled

            // Assign LineRenderer to the AOE attack script
            attack.lineRenderer = lr;

            Debug.Log($"Player '{player.name}' configured successfully.");
        }
    }

    // Helper function to find the damage overlay UI element
    Image FindDamageOverlay()
    {
        GameObject canvasGO = GameObject.Find(canvasName);
        if (canvasGO != null)
        {
            Transform overlayTransform = canvasGO.transform.Find(damageOverlayName);
            if (overlayTransform != null)
            {
                Image overlayImage = overlayTransform.GetComponent<Image>();
                if (overlayImage != null)
                {
                    return overlayImage;
                } else {
                     Debug.LogError($"PlayerBehaviorInjector: Found '{damageOverlayName}' but it has no Image component.", overlayTransform);
                }
            } else {
                 Debug.LogError($"PlayerBehaviorInjector: Cannot find GameObject named '{damageOverlayName}' under Canvas named '{canvasName}'.", canvasGO);
            }
        } else {
             Debug.LogError($"PlayerBehaviorInjector: Cannot find Canvas named '{canvasName}' in scene.");
        }
        return null; // Return null if not found
    }
}