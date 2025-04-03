using UnityEngine;
public class CollectGem : MonoBehaviour
{
    public string skillName = "Fireball"; // Skill name
    public float attackDamage;     // Damage for this skill

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

            // Destroy the gem object after collection
            Destroy(gameObject);
        }
    }
}
