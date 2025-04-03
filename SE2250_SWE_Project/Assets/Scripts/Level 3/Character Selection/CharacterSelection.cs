using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <--- Add this line to use TextMeshPro

public class CharacterSelection : MonoBehaviour
{
    [Header("Character Objects")]
    public GameObject[] characters; // Assign your character prefabs/GameObjects here

    [Header("Character Names")]
    public string[] characterNames; // Assign names corresponding to the characters array

    [Header("UI References")]
    public TextMeshProUGUI characterNameText; // Assign the TextMeshPro UI element here

    public int selectedCharacter = 0;

    void Start()
    {
        // Ensure arrays are valid
        if (characters.Length == 0 || characterNames.Length == 0 || characterNameText == null)
        {
            Debug.LogError("Character Selection setup incomplete. Check arrays and UI references in the Inspector.");
            return; // Stop if setup is wrong
        }
        if (characters.Length != characterNames.Length)
        {
            Debug.LogWarning("Character array and Character Names array have different lengths. This might cause errors.");
        }

        // Activate the initially selected character and deactivate others
        UpdateCharacterDisplay();
    }

    public void NextCharacter()
    {
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        UpdateCharacterDisplay();
    }

    public void PreviousCharacter()
    {
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter = characters.Length - 1; // Correct wrap around
        }
        UpdateCharacterDisplay();
    }

    // Helper function to update the active character and the name display
    void UpdateCharacterDisplay()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            // Activate the selected character, deactivate others
            characters[i].SetActive(i == selectedCharacter);
        }

        // Update the name text, checking bounds
        if (selectedCharacter >= 0 && selectedCharacter < characterNames.Length)
        {
            characterNameText.text = characterNames[selectedCharacter];
        }
        else if (characterNames.Length > 0) // Fallback if index somehow goes out of sync but names exist
        {
             characterNameText.text = characterNames[0]; // Or display an error message
             Debug.LogError($"Selected character index ({selectedCharacter}) is out of bounds for names array (Length: {characterNames.Length}).");
        }
         else
        {
            characterNameText.text = "No Names Assigned"; // Handle case where names array is empty
        }
    }

    public void StartGame()
    {
        // Save the selected character INDEX (as before)
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);

        // Save the selected character NAME
        if (selectedCharacter >= 0 && selectedCharacter < characterNames.Length)
        {
            PlayerPrefs.SetString("selectedCharacterName", characterNames[selectedCharacter]);
            Debug.Log($"Saved character: Index={selectedCharacter}, Name={characterNames[selectedCharacter]}");
        }
        else
        {
            PlayerPrefs.SetString("selectedCharacterName", "DefaultCharacterName"); // Save a default or handle error
            Debug.LogError($"Could not save character name for index {selectedCharacter}. Saving default.");
        }

        // Ensure PlayerPrefs are saved immediately (optional, good practice before scene load)
        PlayerPrefs.Save();

        // Load the next scene (Assuming scene index 1 is your game scene)
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}