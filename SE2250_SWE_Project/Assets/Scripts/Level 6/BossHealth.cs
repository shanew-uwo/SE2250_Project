using UnityEngine;

public class BossHealth : Health
{
    private string[] skills = { "Skill 1", "Skill 2", "Skill 3", "Skill 4" };
    private string weakness = "";
    
    public string playerSkill = "Skill 1";

    public override void Start()
    {
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
        weakness = skills[0];
        Debug.Log("New Boss Weakness: " + weakness);
    }

    private System.Collections.IEnumerator UpdateWeaknessRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
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
}