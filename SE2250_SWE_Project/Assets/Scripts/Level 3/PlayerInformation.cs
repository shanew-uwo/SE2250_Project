using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public int characterType = 7;

    public int CharacterType
    {
        get => characterType;
    }

    public bool hasTalked = false;

    public bool HasTalked
    {
        get => hasTalked;
    }

    public void SetHasTalked()
    {
        hasTalked = true;
    }
}
