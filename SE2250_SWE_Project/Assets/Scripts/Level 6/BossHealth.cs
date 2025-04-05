using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Ensure this inherits from YOUR refactored base Health script
public class BossHealth : Health
{
    private string[] skills = { "Skill 1", "Skill 2", "Skill 3", "Skill 4" };
    private string weakness = "";

    [Header("Boss Specifics")] // Added Header for clarity
    public string playerSkill = "Skill 1"; // This should probably be updated by PlayerAOEAttack
    public TMP_Text weaknessText;
    [SerializeField] private float actualMaxBossHealth = 1000f; // Renamed to avoid hiding base maxHealth

    public override void Start()
    {
        // Use the CORRECT SetMaxHealth from the base class
        SetMaxHealth(actualMaxBossHealth, true);
        // Call base.Start() AFTER setting max health
        base.Start();

        UpdateWeakness(); // Set initial weakness
        // Ensure the coroutine doesn't break if weaknessText is null
        if (weaknessText != null)
        {
            StartCoroutine(UpdateWeaknessRoutine());
        } else {
            Debug.LogError("BossHealth: Weakness Text (TMP_Text) is not assigned!", this);
        }
    }

    // Remove base.Update() call if the base Update doesn't do anything critical for Boss
    // public override void Update()
    // {
    //     base.Update();
    // }

    public void UpdateWeakness()
    {
        int index = Random.Range(0, skills.Length);
        weakness = skills[index];
        // Debug.Log("New Boss Weakness: " + weakness); // Optional

        // Only update text if it's assigned
        if (weaknessText != null)
        {
            switch (index)
            {
                case 0: weaknessText.SetText("JS"); break;
                case 1: weaknessText.SetText("Python"); break;
                case 2: weaknessText.SetText("Java"); break;
                case 3: weaknessText.SetText("C#"); break;
            }
        }
    }

    private System.Collections.IEnumerator UpdateWeaknessRoutine()
    {
        // Prevent running if text is missing
        if (weaknessText == null) yield break;

        while (true) // Loop indefinitely while the boss is alive
        {
            yield return new WaitForSeconds(3f);
             // Check if health > 0 before updating, coroutine might run after death otherwise
            if (currentHealth > 0)
            {
                UpdateWeakness();
            } else {
                 yield break; // Stop coroutine if boss died
            }
        }
    }

    public override void TakeDamage(float amount)
    {
        float modifiedAmount = amount; // Start with original amount
        // Apply weakness modifier
        if (playerSkill == weakness)
        {
            modifiedAmount *= 2;
            Debug.Log($"Boss Weakness {weakness} Hit! Damage multiplied.");
        } else {
            Debug.Log($"Boss Weakness {weakness} NOT Hit (Player Skill: {playerSkill}). Normal damage.");
        }

        // Call the base TakeDamage with the potentially modified amount
        base.TakeDamage(modifiedAmount);
    }

    // Override PerformDeathAction instead of Die
    protected override void PerformDeathAction()
    {
        Debug.Log($"Boss '{name}' died. Loading completion scene.");
        // Stop weakness updates
        StopAllCoroutines();
        // Ensure object exists before loading scene
        if (this != null && gameObject != null)
        {
            SceneManager.LoadScene("Level6BossCompleted");
        }
        // Do not call base.PerformDeathAction() as that reloads the current scene
    }
}