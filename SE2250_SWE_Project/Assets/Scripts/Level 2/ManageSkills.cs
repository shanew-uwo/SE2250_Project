using UnityEngine;
using System.Collections.Generic;
public class ManageSkills : MonoBehaviour
{
    // List of all skills the player has collected
    private List<Skill> playerSkills = new List<Skill>();
    private List<string> requiredSkills = new List<string> { "JavaScript", "Git", "Database Management", "Cloud Computing" };  // List of required skills to advance

    // Player stats
    private float currentAttackDamage;

    // Method to add a skill to the player
    public void AddSkill(Skill skill)
    {
        playerSkills.Add(skill);
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
    
    // Method to check if the player has all required skills
    public bool HasAllRequiredSkills()
    {
        foreach (var requiredSkill in requiredSkills)
        {
            bool hasSkill = false;

            // Check if player has the required skill
            foreach (var playerSkill in playerSkills)
            {
                if (playerSkill.skillName == requiredSkill)
                {
                    hasSkill = true;
                    break;
                }
            }

            if (!hasSkill)  // If any required skill is missing
            {
                return false;
            }
        }
        return true;  // All required skills are present
    }

    // For testing purposes, get the player's current attack damage
    public float GetCurrentAttackDamage()
    {
        return currentAttackDamage;
    }
    
    public List<Skill> GetSkills()
    {
        return new List<Skill>(playerSkills);
    }

}
