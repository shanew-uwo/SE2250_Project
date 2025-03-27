using UnityEngine;
using System.Collections.Generic;
public class ManageSkills : MonoBehaviour
{
    // List of all skills the player has collected
    private List<Skill> playerSkills = new List<Skill>();

    // Player stats
    private float currentAttackDamage;

    // Method to add a skill to the player
    public void AddSkill(Skill skill)
    {
        playerSkills.Add(skill);
        Debug.Log("Player has gained a new skill: " + skill.skillName);
        ApplySkillEffect(skill);
    }

    // Method to apply the effect of a granted skill
    private void ApplySkillEffect(Skill skill)
    {
        switch (skill.skillName)
        {
            case "JavaScript":
                currentAttackDamage = 1f;  
                Debug.Log("JavaScript acquired, attack damage: 1");
                break;
            case "Git":
                currentAttackDamage = 5f;
                Debug.Log("Git acquired, attack damage: 5");
                break;
            case "Database Management":
                currentAttackDamage = 7f;
                Debug.Log("Database Management acquired, attack damage: 7");
                break;
            default:
                currentAttackDamage = 10f;  
                Debug.Log("Cloud Computing acquired, attack damage: 10");
                break;
        }
    }


    // For testing purposes, get the player's current attack damage
    public float GetCurrentAttackDamage()
    {
        return currentAttackDamage;
    }

}
