using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossHealth : Health
{
    private string[] skills = { "Skill 1", "Skill 2", "Skill 3", "Skill 4" };
    private string weakness = "";
    
    public string playerSkill = "Skill 1";
    public TMP_Text weaknessText;
    public float maxBossHealth = 1000f;

    public override void Start()
    {
        base.setMaxHealth(maxHealth);
        base.Start();
        UpdateWeakness(); // Set initial weakness
        StartCoroutine(UpdateWeaknessRoutine());
    }

    public override void Update()
    {
        base.Update();
    }

    public void UpdateWeakness()
    {
        int index = Random.Range(0, skills.Length);
        weakness = skills[index];
        Debug.Log("New Boss Weakness: " + weakness);

        switch (index)
        {
            case 0:
                weaknessText.SetText("JS");
                break;
            case 1:
                weaknessText.SetText("Python");
                break;
            case 2:
                weaknessText.SetText("Java");
                break;
            case 3:
                weaknessText.SetText("C#");
                break;
        }
    }

    private System.Collections.IEnumerator UpdateWeaknessRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            UpdateWeakness();
        }
    }

    public override void TakeDamage(float amount)
    {
        if (playerSkill == weakness)
        {
            amount *= 2;
        }
        
        base.TakeDamage(amount);
    }

    protected override void Die()
    {
        SceneManager.LoadScene("Level6BossCompleted");
    }
}