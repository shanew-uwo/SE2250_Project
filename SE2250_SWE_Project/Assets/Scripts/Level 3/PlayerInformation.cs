using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public int characterType = 0;

    public int CharacterType
    {
        get => characterType;
    }

    private bool hasTalked = false;

    public bool HasTalked
    {
        get => hasTalked;
    }

    public void SetHasTalked()
    {
        hasTalked = true;
    }
}
