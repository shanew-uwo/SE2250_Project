using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // 1. Add this namespace for TextMeshPro

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters;
    public int selectedCharacter = 0;

    // 2. Add a public variable to hold the reference to your TextMeshProUGUI element
    public TextMeshProUGUI characterNameText;

    // Use Start to set the initial character name when the scene loads
    void Start()
    {
        // Ensure only the selected character is active initially (good practice)
        for(int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == selectedCharacter);
        }

        // Update the text display for the initially selected character
        UpdateCharacterInfo();
    }

    public void NextCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        characters[selectedCharacter].SetActive(true);

        // 4. Update the text display
        UpdateCharacterInfo();
    }

    public void PreviousCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
        }
        characters[selectedCharacter].SetActive(true);

        // 4. Update the text display
        UpdateCharacterInfo();
    }

    // 3. Create a function to update the text
    void UpdateCharacterInfo()
    {
        if (characterNameText != null && characters.Length > 0) // Check if references are set
        {
            // Get the name directly from the GameObject in the array
            string currentName = characters[selectedCharacter].name;
            characterNameText.text = currentName;
        }
        else
        {
             if(characterNameText == null)
                Debug.LogError("CharacterNameText is not assigned in the inspector!");
             if(characters.Length == 0)
                Debug.LogError("Characters array is empty!");
        }
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        // Optional: You could also save the name if needed in the next scene
        // PlayerPrefs.SetString("selectedCharacterName", characters[selectedCharacter].name);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}