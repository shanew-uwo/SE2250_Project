using UnityEngine;
using UnityEngine.UI; // Need this for Image

public class PlayerBehaviorInjector : MonoBehaviour
{
    [Header("Player Component Settings")]
    [SerializeField] private float playerMaxHealth = 100f;
    [SerializeField] private string playerTag = "Player";

    [Header("AOE Attack Settings")]
    [SerializeField] private float playerAoeRadius = 5f;
    [SerializeField] private float playerAoeDamage = 25f;
    [SerializeField] private float playerAoeCooldown = 0.7f;
    [SerializeField] private string playerAoeTargetTag = "Bot"; // *** IMPORTANT: Match Spawner/Recruiter ***
    [SerializeField] private KeyCode playerAoeKey = KeyCode.Q;

    [Header("AOE Visuals")]
    public Gradient greenGradient; // Assign in Inspector
    public Gradient orangeGradient; // Assign in Inspector
    public Gradient purpleGradient; // Assign in Inspector
    public Gradient blueGradient; // Assign in Inspector
    public Material lineMaterial; // Assign LineRenderer Material in Inspector

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

            // --- Remove Old Script ---
            KillingBots oldInteraction = player.GetComponent<KillingBots>();
            if (oldInteraction != null)
            {
                Debug.Log($"Removing old KillingBots script from {player.name}.");
                Destroy(oldInteraction);
            }

            // --- Ensure Health Component ---
            Health health = player.GetComponent<Health>();
            if (health == null)
            {
                health = player.AddComponent<Health>();
                 Debug.Log($"Added Health component to {player.name}.");
            }
            health.SetMaxHealth(playerMaxHealth, true); // Set max health
            health.damageOverlay = damageOverlayImage; // Assign UI overlay

            // --- Ensure PlayerAOEAttack Component ---
            PlayerAOEAttack attack = player.GetComponent<PlayerAOEAttack>();
            if (attack == null)
            {
                attack = player.AddComponent<PlayerAOEAttack>();
                Debug.Log($"Added PlayerAOEAttack component to {player.name}.");
            }

            // Configure AOE Attack settings
            attack.aoeRadius = playerAoeRadius;
            attack.aoeDamage = playerAoeDamage;
            attack.cooldown = playerAoeCooldown;
            attack.enemyTag = playerAoeTargetTag; // Set the target tag
            // attack.attackKey = playerAoeKey; // Set the attack key (Script already does this via SerializeField now)


            // Assign AOE Color Gradients (ensure array exists)
            if (attack.aoeColors == null || attack.aoeColors.Length != 4) {
                 attack.aoeColors = new Gradient[4];
            }
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
            lr.material = lineMaterial; // Assign the material
            lr.positionCount = 0; // Start with 0 points until AOE is used
            lr.enabled = false; // Start disabled

            // Assign LineRenderer to the AOE attack script
            attack.lineRenderer = lr;

             Debug.Log($"Player '{player.name}' configured successfully.");
        }
    }

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
                 Debug.LogError($"PlayerBehaviorInjector: Cannot find GameObject named '{damageOverlayName}' under '{canvasName}'.", canvasGO);
            }
        } else {
             Debug.LogError($"PlayerBehaviorInjector: Cannot find Canvas named '{canvasName}' in scene.");
        }
        return null; // Not found
    }
}