using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviorInjector : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float playerMaxHealth = 100f;

    [Header("AOE Attack Settings")]
    [SerializeField] private float playerAoeRadius = 5f;
    [SerializeField] private float playerAoeDamage = 25f;
    [SerializeField] private float playerAoeCooldown = 0.7f;
    [SerializeField] private string[] playerAoeTargetTags = { "Bot", "Enemy", "Boss" };
    [SerializeField] private KeyCode playerAoeKey = KeyCode.Q;

    [Header("AOE Visuals")]
    public Gradient greenGradient;
    public Gradient orangeGradient;
    public Gradient purpleGradient;
    public Gradient blueGradient;
    public Material lineMaterial;

    [Header("UI Settings")]
    [SerializeField] private string canvasName = "Canvas";
    [SerializeField] private string damageOverlayName = "DamageOverlay";

    void Start()
    {
        StartCoroutine(WaitForPlayerAndConfigure());
    }

    private System.Collections.IEnumerator WaitForPlayerAndConfigure()
    {
        GameObject[] players = null;

        // Wait until a player is found
        while (players == null || players.Length == 0)
        {
            players = GameObject.FindGameObjectsWithTag(playerTag);
            yield return null;
        }

        yield return null; // Extra frame for safety

        ConfigurePlayers(players);
    }

    private void ConfigurePlayers(GameObject[] players)
    {
        Image damageOverlayImage = FindDamageOverlay();

        foreach (GameObject player in players)
        {
            Debug.Log($"[Injector] Configuring player '{player.name}'...");

            // Remove old script if present
            var oldScript = player.GetComponent("KillingBots");
            if (oldScript != null)
            {
                Destroy(oldScript);
                Debug.Log($"[Injector] Removed old script from '{player.name}'.");
            }

            // Add or configure Health
            Health health = player.GetComponent<Health>();
            if (health == null)
            {
                health = player.AddComponent<Health>();
                Debug.Log($"[Injector] Added Health to '{player.name}'.");
            }

            health.SetMaxHealth(playerMaxHealth, true);
            health.damageOverlay = damageOverlayImage;

            // Add or configure AOE attack
            PlayerAOEAttack attack = player.GetComponent<PlayerAOEAttack>();
            if (attack == null)
            {
                attack = player.AddComponent<PlayerAOEAttack>();
                Debug.Log($"[Injector] Added PlayerAOEAttack to '{player.name}'.");
            }

            attack.aoeRadius = playerAoeRadius;
            attack.aoeDamage = playerAoeDamage;
            attack.cooldown = playerAoeCooldown;
            attack.targetTags = playerAoeTargetTags;

            attack.aoeColors = new Gradient[4]
            {
                greenGradient,
                orangeGradient,
                purpleGradient,
                blueGradient
            };

            // Add or configure LineRenderer
            LineRenderer lr = player.GetComponent<LineRenderer>();
            if (lr == null)
            {
                lr = player.AddComponent<LineRenderer>();
                Debug.Log($"[Injector] Added LineRenderer to '{player.name}'.");
            }

            lr.useWorldSpace = true;
            lr.loop = true;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material = lineMaterial;
            lr.positionCount = 0;
            lr.enabled = false;

            attack.lineRenderer = lr;

            Debug.Log($"[Injector] Finished configuring '{player.name}'.");
        }
    }

    private Image FindDamageOverlay()
    {
        GameObject canvas = GameObject.Find(canvasName);
        if (canvas != null)
        {
            Transform overlay = canvas.transform.Find(damageOverlayName);
            if (overlay != null)
            {
                Image img = overlay.GetComponent<Image>();
                if (img != null)
                    return img;

                Debug.LogError($"[Injector] '{damageOverlayName}' found but missing Image component.");
            }
            else
            {
                Debug.LogError($"[Injector] Could not find '{damageOverlayName}' under '{canvasName}'.");
            }
        }
        else
        {
            Debug.LogError($"[Injector] Canvas '{canvasName}' not found.");
        }

        return null;
    }
}
