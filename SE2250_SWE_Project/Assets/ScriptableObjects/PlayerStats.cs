using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Custom/Player Stats")]
public class PlayerStats : ScriptableObject
{
    public float jumpForce = 5f;

    public void IncreaseJumpForce(float amount)
    {
        jumpForce += amount;
        Debug.Log("Jump Force increased by " + amount + ". New value: " + jumpForce);
    }
}