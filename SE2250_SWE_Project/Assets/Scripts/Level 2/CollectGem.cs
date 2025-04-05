using System.Collections;
using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class CollectGem : MonoBehaviour
{
    public string skillName; // Skill name
    public float attackDamage;           // Damage for this skill
    public TextMeshProUGUI skillText;    // Reference to the TextMeshProUGUI component

    // Change from OnCollisionEnter to OnTriggerEnter for Trigger colliders
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is the player
        if (other.CompareTag("Player"))
        {
            // Create a new skill object with the skill name and damage value
            Skill newSkill = new Skill(skillName, attackDamage);

            // Get the SkillManager component attached to the player
            ManageSkills skillManager = other.GetComponent<ManageSkills>();
            if (skillManager != null)
            {
                skillManager.AddSkill(newSkill);
            }

            // Update the UI text to show the skill collected message
            if (skillText != null)
            {
                skillText.text = "Skill gained: " + skillName +"!";
            }

            // Start the coroutine to hide the text after 5 seconds
            StartCoroutine(HideSkillTextAfterDelay(3f));

            // Destroy the gem object after collection
            Destroy(gameObject);
        }
    }

    // Coroutine to hide the text after a delay
    private IEnumerator HideSkillTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified time
        if (skillText != null)
        {
            skillText.text = "";  // Clear the text
        }
    }
}


