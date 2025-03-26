using UnityEngine;
public class CollectGem : MonoBehaviour
{
    // Reference to the player's skill manager (this assumes your player has a SkillManager script)
    public string skillToGrant = "SpeedBoost";  // Example skill name

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player (make sure your player is tagged "Player")
        if (collision.gameObject.CompareTag("Player"))
        {
            // Grant the skill to the player
            ManageSkills skillManager = collision.gameObject.GetComponent<ManageSkills>();
            if (skillManager != null)
            {
                skillManager.AddSkill(skillToGrant);
            }

            // Destroy the gem object
            Destroy(gameObject);
        }
    }
}
