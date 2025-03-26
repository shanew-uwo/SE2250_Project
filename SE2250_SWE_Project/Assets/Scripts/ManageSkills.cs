using UnityEngine;
using System.Collections.Generic;
public class ManageSkills : MonoBehaviour
{
    // List to store skills the player has
    private List<string> playerSkills = new List<string>();

    // Method to add a skill to the player
    public void AddSkill(string skill)
    {
        // Check if the player already has this skill
        if (!playerSkills.Contains(skill))
        {
            playerSkills.Add(skill);
            Debug.Log("Player has gained a new skill: " + skill);
        }
        else
        {
            Debug.Log("Player already has this skill: " + skill);
        }
    }

}
