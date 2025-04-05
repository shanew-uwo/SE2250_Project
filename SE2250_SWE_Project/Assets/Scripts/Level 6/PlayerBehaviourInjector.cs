using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class PlayerBehaviorInjector : MonoBehaviour
{
    public Gradient greenGradient;
    public Gradient orangeGradient;
    public Gradient purpleGradient;
    public Gradient blueGradient;

    public Material lineMaterial; // Assign this in the Inspector

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Find the UIManager object and HealthBarUI script (optional but recommended)
        GameObject uiManagerGO = GameObject.Find("UIManager");
        HealthBarUI healthBarUI = null;
        if (uiManagerGO != null)
        {
            healthBarUI = uiManagerGO.GetComponent<HealthBarUI>();
        }

        foreach (GameObject player in players)
        {
            PlayerAOEAttack attack = player.GetComponent<PlayerAOEAttack>();
            if (attack == null)
            {
                attack = player.AddComponent<PlayerAOEAttack>();
            }

            // Assign Gradients
            attack.aoeColors = new Gradient[4];
            attack.aoeColors[0] = greenGradient;
            attack.aoeColors[1] = orangeGradient;
            attack.aoeColors[2] = purpleGradient;
            attack.aoeColors[3] = blueGradient;

            // Add LineRenderer if not present
            LineRenderer lr = player.GetComponent<LineRenderer>();
            if (lr == null)
            {
                lr = player.AddComponent<LineRenderer>();
            }

            // Configure LineRenderer
            lr.useWorldSpace = true;
            lr.loop = true;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material = lineMaterial;
            lr.enabled = false;

            attack.lineRenderer = lr;

            // Add or get Health component
            Health health = player.GetComponent<Health>();
            if (health == null)
            {
                health = player.AddComponent<Health>();
            }

            // Assign damage overlay
            GameObject canvas = GameObject.Find("Canvas");
            Transform overlayTransform = canvas?.transform.Find("DamageOverlay");
            if (overlayTransform != null)
            {
                Image damageOverlay = overlayTransform.GetComponent<Image>();
                health.damageOverlay = damageOverlay;
            }
            else
            {
                Debug.LogWarning("DamageOverlay image not found under Canvas.");
            }

            // 👉 Assign to UIManager’s HealthBarUI
            if (healthBarUI != null)
            {
                healthBarUI.playerHealth = health;
            }
            else
            {
                Debug.LogWarning("UIManager or HealthBarUI not found. Player health bar not assigned.");
            }
        }
    }
}
