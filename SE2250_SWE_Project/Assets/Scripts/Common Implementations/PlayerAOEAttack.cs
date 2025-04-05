using UnityEngine;
using System.Collections; // Required for IEnumerator

public class PlayerAOEAttack : MonoBehaviour
{
    [Header("AOE Settings")]
    public float aoeRadius = 5f;
    public float aoeDamage = 20f;
    public float cooldown = 0.5f;
    [Tooltip("The tag of enemies/bots this AOE should damage.")]
    [SerializeField] public string enemyTag = "Bot"; // << MAKE SURE THIS MATCHES SPAWNER/RECRUITER

    [Header("Input")]
    [SerializeField] private KeyCode attackKey = KeyCode.Q; // Changed from hardcoded 'Q'

    [Header("Line Renderer Settings")]
    public LineRenderer lineRenderer; // Assign via Injector
    public int lineSegments = 50;
    public float lineDuration = 0.3f;

    [Header("AOE Color Presets (Optional)")]
    public Gradient[] aoeColors = new Gradient[4]; // Assign via Injector
    // public string[] skillNames = {"Skill 1", "Skill 2", "Skill 3", "Skill 4"}; // Optional UI tie-in

    private Gradient currentGradient;
    private float cooldownTimer = 0f;
    // private int currentIndex = 0; // Keep if using color switching

    void Start()
    {
        // Default to the first color if available and assigned
        if (aoeColors != null && aoeColors.Length > 0 && aoeColors[0] != null) {
            currentGradient = aoeColors[0];
        } else if (lineRenderer != null) {
            // Fallback if no gradients assigned but renderer exists
            currentGradient = new Gradient(); // Simple white fallback
            currentGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
        }

        if (lineRenderer != null) lineRenderer.enabled = false; // Ensure it starts disabled
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        // Use the configurable attackKey
        if (Input.GetKeyDown(attackKey) && cooldownTimer <= 0f)
        {
            PerformAOE();
            cooldownTimer = cooldown;
        }

        // --- Optional: Color Switching Logic ---
        // if (Input.GetKeyDown(KeyCode.Alpha1)) SetAOEColor(0);
        // ... etc ...
    }

    // Optional: Method for color switching
    // void SetAOEColor(int index) { ... }

    void PerformAOE()
    {
        Debug.Log($"Player performing AOE attack (Key: {attackKey}, Radius: {aoeRadius}, Damage: {aoeDamage}, Target Tag: '{enemyTag}')");
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
        int enemiesHit = 0;

        foreach (Collider hit in hits)
        {
            // Check against the configured enemyTag
            if (hit.CompareTag(enemyTag))
            {
                // Use the base Health component
                Health health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(aoeDamage);
                    enemiesHit++;
                     Debug.Log($"Player AOE hit: {hit.name}");
                }
                else
                {
                     Debug.LogWarning($"Player AOE hit {hit.name} (Tag: {enemyTag}) but it has no Health component!", hit.gameObject);
                }
            }
        }

         Debug.Log($"Player AOE finished. Hit {enemiesHit} entities tagged '{enemyTag}'.");

        // Trigger visual effect
        StartCoroutine(DrawAOECircle());
    }

    IEnumerator DrawAOECircle() // Changed return type to IEnumerator
    {
        if (lineRenderer == null || currentGradient == null) {
            Debug.LogWarning("Cannot draw AOE circle - LineRenderer or CurrentGradient is null.", this);
            yield break; // Exit coroutine if components are missing
        }


        lineRenderer.positionCount = lineSegments + 1;
        lineRenderer.colorGradient = currentGradient; // Apply the gradient

        float angleStep = 360f / lineSegments;
        for (int i = 0; i <= lineSegments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            float x = transform.position.x + Mathf.Cos(angle) * aoeRadius;
            float z = transform.position.z + Mathf.Sin(angle) * aoeRadius;
            // Ensure Y position is slightly above the player's base if desired
            Vector3 point = new Vector3(x, transform.position.y + 0.1f, z);
            lineRenderer.SetPosition(i, point);
        }

        lineRenderer.enabled = true;
        yield return new WaitForSeconds(lineDuration);

        // Check if lineRenderer still exists before disabling (might be destroyed)
        if (lineRenderer != null) {
             lineRenderer.enabled = false;
        }
    }

    // Keep Gizmos for easy visualization in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; // Changed color to distinguish from enemy AOE
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}