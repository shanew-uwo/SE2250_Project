using UnityEngine;

public class PlayerAOEAttack : MonoBehaviour
{
    [Header("AOE Settings")]
    public float aoeRadius = 5f;
    public float aoeDamage = 20f;
    public float cooldown = 0.5f;

    [Header("Line Renderer Settings")]
    public LineRenderer lineRenderer;
    public int lineSegments = 50;
    public float lineDuration = 0.3f;

    [Header("AOE Color Presets")]
    public Gradient[] aoeColors = new Gradient[4]; // Set in Inspector
    public string[] skillNames = {"Skill 1", "Skill 2", "Skill 3", "Skill 4"};

    private Gradient currentGradient;
    private float cooldownTimer = 0f;
    private string currentSkill = "Skill 1";
    
    private BossHealth bossHealth;

    void Start()
    {
        // Default to the first color if available
        if (aoeColors.Length > 0)
            currentGradient = aoeColors[0];
        
        bossHealth = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossHealth>();
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q) && cooldownTimer <= 0f)
        {
            PerformAOE();
            cooldownTimer = cooldown;
        }

        // Change color presets on number key press
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetAOEColor(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetAOEColor(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetAOEColor(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetAOEColor(3);
    }

    void SetAOEColor(int index)
    {
        if (index >= 0 && index < aoeColors.Length)
        {
            currentGradient = aoeColors[index];
            if (bossHealth != null)
            {
                bossHealth.playerSkill = skillNames[index];
                Debug.Log(bossHealth.playerSkill);
            }

            Debug.Log("AOE color changed to preset #" + (index + 1));
        }
    }

    void PerformAOE()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy") || hit.CompareTag("Boss"))
            {
                Health health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(aoeDamage);
                }
            }
        }

        StartCoroutine(DrawAOECircle());
    }

    System.Collections.IEnumerator DrawAOECircle()
    {
        if (lineRenderer == null || currentGradient == null) yield break;

        lineRenderer.positionCount = lineSegments + 1;
        lineRenderer.colorGradient = currentGradient;

        float angleStep = 360f / lineSegments;
        for (int i = 0; i <= lineSegments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            float x = transform.position.x + Mathf.Cos(angle) * aoeRadius;
            float z = transform.position.z + Mathf.Sin(angle) * aoeRadius;
            Vector3 point = new Vector3(x, transform.position.y + 0.1f, z);
            lineRenderer.SetPosition(i, point);
        }

        lineRenderer.enabled = true;
        yield return new WaitForSeconds(lineDuration);
        lineRenderer.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
