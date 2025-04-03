using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Custom/Player Stats")]
public class PlayerStats : ScriptableObject
{
    public float defaultJumpForce = 5f;
    [HideInInspector]
    public float runtimeJumpForce;

    public void ResetRuntimeValues()
    {
        runtimeJumpForce = defaultJumpForce;
    }

    public void IncreaseJumpForce(float amount)
    {
        runtimeJumpForce += amount;
        Debug.Log("Jump Force increased by " + amount + ". New value: " + runtimeJumpForce);
    }
}