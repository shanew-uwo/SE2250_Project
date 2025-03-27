using UnityEngine;
public class CollectGem : MonoBehaviour
{
    public string skillName = "Fireball"; // Skill name
    public float attackDamage;     // Damage for this skill

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Create a new skill object with the skill name and damage value
            Skill newSkill = new Skill(skillName, attackDamage);

            // Get the SkillManager component attached to the player
            ManageSkills skillManager = collision.gameObject.GetComponent<ManageSkills>();
            if (skillManager != null)
            {
                skillManager.AddSkill(newSkill);
            }

            // Destroy the gem object
            Destroy(gameObject);
        }
    }
}
