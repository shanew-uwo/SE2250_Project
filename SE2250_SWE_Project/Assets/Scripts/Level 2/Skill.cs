using UnityEngine;

public class Skill
{
    public string skillName;
    public float attackDamage;
    
    public Skill(string name, float damage)
    {
        skillName = name;
        attackDamage = damage;
    }
}
