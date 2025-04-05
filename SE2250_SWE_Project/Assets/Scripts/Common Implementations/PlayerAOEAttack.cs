using UnityEngine;
using System.Collections;
using System.Linq; // *** ADDED for Enumerable.Contains() ***

public class PlayerAOEAttack : MonoBehaviour
{
    [Header("AOE Settings")]
    [Tooltip("Radius of the AOE attack.")]
    public float aoeRadius = 5f;
    [Tooltip("Damage dealt by the AOE attack.")]
    public float aoeDamage = 20f;
    [Tooltip("Cooldown in seconds between AOE attacks.")]
    public float cooldown = 0.5f;

    // *** CHANGED: Field to receive multiple tags from Injector ***
    [Tooltip("Tags that this AOE attack will damage (set by PlayerBehaviorInjector).")]
    [HideInInspector] // Hide from Inspector as Injector sets it
    public string[] targetTags = {"Bot", "Enemy", "Boss"}; // Default value, Injector overrides
    // *** END CHANGE ***

    [Header("Input")]
    [Tooltip("Key to trigger the AOE attack.")]
    [SerializeField] private KeyCode attackKey = KeyCode.Q;

    [Header("Line Renderer Settings")]
    [Tooltip("Assign the LineRenderer component (usually assigned by PlayerBehaviorInjector).")]
    public LineRenderer lineRenderer;
    [Tooltip("Number of segments for the visual effect circle.")]
    public int lineSegments = 50;
    [Tooltip("How long the visual effect circle stays visible (in seconds).")]
    public float lineDuration = 0.3f;

    [Header("AOE Color Presets (Optional)")]
    [Tooltip("Assign Gradient assets for different skill visuals (usually assigned by PlayerBehaviorInjector).")]
    public Gradient[] aoeColors = new Gradient[4];
    [Tooltip("Names corresponding to the skill/color presets (used for Boss Weakness link).")]
    public string[] skillNames = {"Skill 1", "Skill 2", "Skill 3", "Skill 4"};

    // --- Private Variables ---
    private Gradient currentGradient;
    private float cooldownTimer = 0f;
    private BossHealth bossHealth; // Cached reference if linking to boss weakness

    void Start()
    {
        // Set default gradient for visual effect
        if (aoeColors != null && aoeColors.Length > 0 && aoeColors[0] != null) {
            currentGradient = aoeColors[0];
        } else if (lineRenderer != null) {
            // Fallback if no gradients assigned but renderer exists
            currentGradient = new Gradient(); // Simple white fallback
            currentGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
             Debug.LogWarning("PlayerAOEAttack: No color gradients assigned, using default white.", this);
        }

        // Ensure LineRenderer starts disabled
        if (lineRenderer != null) lineRenderer.enabled = false;

        // Attempt to find BossHealth if using skill weakness mechanic
        // This assumes the Boss GameObject is tagged "Boss"
        GameObject bossObj = GameObject.FindGameObjectWithTag("Boss");
        if (bossObj != null) {
            bossHealth = bossObj.GetComponent<BossHealth>();
             // Initialize BossHealth with player's starting skill if applicable
            if (bossHealth != null && skillNames != null && skillNames.Length > 0) {
                bossHealth.playerSkill = skillNames[0];
            }
        }
         // Optional: Warn if Boss expected but not found
         // else { Debug.LogWarning("PlayerAOEAttack: Could not find GameObject tagged 'Boss' for skill weakness link."); }
    }

    void Update()
    {
        // Handle Cooldown
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        // Handle Attack Input
        if (Input.GetKeyDown(attackKey) && cooldownTimer <= 0f)
        {
            PerformAOE();
            cooldownTimer = cooldown; // Reset cooldown
        }

        // Handle Skill/Color Switching Input (Optional)
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetAOEColorAndSkill(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetAOEColorAndSkill(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetAOEColorAndSkill(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetAOEColorAndSkill(3);
    }

    // Optional: Method for color/skill switching
    void SetAOEColorAndSkill(int index)
    {
        // Check if index is valid for both arrays
        if (index >= 0 && aoeColors != null && index < aoeColors.Length && skillNames != null && index < skillNames.Length)
        {
            currentGradient = aoeColors[index]; // Set visual gradient

            // Update BossHealth's knowledge of the player's current skill
            if (bossHealth != null)
            {
                bossHealth.playerSkill = skillNames[index];
                Debug.Log($"Player Skill changed to: {bossHealth.playerSkill}");
            }
            Debug.Log("AOE color/skill changed to preset #" + (index + 1));
        } else {
             Debug.LogWarning($"SetAOEColorAndSkill: Invalid index {index} or arrays not configured correctly.");
        }
    }

    // Performs the AOE damage and visual effect
    void PerformAOE()
    {
        string tagsList = (targetTags != null && targetTags.Length > 0) ? $"[{string.Join(", ", targetTags)}]" : "NONE ASSIGNED";
        Debug.Log($"Player performing AOE attack (Key: {attackKey}, Radius: {aoeRadius}, Damage: {aoeDamage}, Target Tags: {tagsList})");

        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
        int targetsHit = 0;

        // Ensure targetTags array is valid before checking
        if (targetTags == null || targetTags.Length == 0)
        {
            Debug.LogWarning("PlayerAOEAttack: No target tags assigned by Injector. AOE will not hit anything.", this);
        }
        else
        {
            // Iterate through all objects hit by the OverlapSphere
            foreach (Collider hit in hits)
            {
                // *** CHANGED: Check if the hit object's tag is present in the targetTags array ***
                if (targetTags.Contains(hit.tag))
                {
                    // Try to get the Health component from the hit object
                    Health health = hit.GetComponent<Health>();
                    if (health != null)
                    {
                        // Apply damage using the base Health script's method
                        health.TakeDamage(aoeDamage);
                        targetsHit++;
                        Debug.Log($"Player AOE hit: {hit.name} (Tag: {hit.tag})");
                    }
                    else
                    {
                        // Log a warning if the object has the right tag but no Health script
                        Debug.LogWarning($"Player AOE hit {hit.name} (Tag: {hit.tag}) but it has no Health component!", hit.gameObject);
                    }
                }
                // *** END CHANGE ***
            }
        }

        Debug.Log($"Player AOE finished. Hit {targetsHit} entities with matching tags.");

        // Trigger the visual effect coroutine
        StartCoroutine(DrawAOECircle());
    }

    // Coroutine to draw the expanding/fading circle visual
    IEnumerator DrawAOECircle()
    {
        // Don't run if essential components are missing
        if (lineRenderer == null || currentGradient == null) {
            Debug.LogWarning("Cannot draw AOE circle - LineRenderer or CurrentGradient is null.", this);
            yield break;
        }

        // Setup LineRenderer
        lineRenderer.positionCount = lineSegments + 1;
        lineRenderer.colorGradient = currentGradient; // Apply the selected gradient

        // Calculate and set points for the circle
        float angleStep = 360f / lineSegments;
        for (int i = 0; i <= lineSegments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            float x = transform.position.x + Mathf.Cos(angle) * aoeRadius;
            float z = transform.position.z + Mathf.Sin(angle) * aoeRadius;
            // Add a small Y offset to ensure visibility on flat ground
            Vector3 point = new Vector3(x, transform.position.y + 0.1f, z);
            lineRenderer.SetPosition(i, point);
        }

        // Show the circle and fade it out
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(lineDuration); // Wait for specified duration

        // Hide the circle after duration (check if it still exists)
        if (lineRenderer != null) {
             lineRenderer.enabled = false;
        }
    }

    // Draws the AOE radius gizmo in the Scene view when the object is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; // Use a distinct color for player AOE gizmo
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}