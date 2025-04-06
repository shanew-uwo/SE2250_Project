using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public int characterType = 0;

    public int CharacterType
    {
        get => characterType;
    }

    private static bool  _hasTalked = false;

    public static bool HasTalked
    {
        get => _hasTalked;
    }

    public void SetHasTalked()
    {
        _hasTalked = true;
    }
}
