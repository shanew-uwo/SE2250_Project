using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    // Set this value in the Inspector or through game logic
    // 0 = The type that triggers the specific dialogue path in your screenshot
    // 1 = The default type (or another type)
    public int characterType = 0;

    // You can add other player-related data here if needed,
    // like inventory, quest status, etc.
    // public bool hasCompletedGuardQuest = false;

    public int CharacterType => characterType;
}