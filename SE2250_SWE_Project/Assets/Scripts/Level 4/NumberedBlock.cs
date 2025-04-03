using UnityEngine;
using TMPro;

public class NumberedBlock : MonoBehaviour
{
    [Header("Block Info")]
    public int blockNumber;                     // The number you assign in the Inspector

    [Header("Visuals")]
    public TMP_Text numberLabel;             // Drag in the child TextMeshPro object

    void Start()
    {
        if (numberLabel != null)
        {
            numberLabel.text = blockNumber.ToString();
        }
    }

    // Optional: Call this if you want to update number later
    public void SetNumber(int newNumber)
    {
        blockNumber = newNumber;
        if (numberLabel != null)
        {
            numberLabel.text = blockNumber.ToString();
        }
    }

    public int GetNumber()
    {
        return blockNumber;
    }
}