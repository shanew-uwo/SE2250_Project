using UnityEngine;
using TMPro;  // Include the TextMeshPro namespace
using UnityEngine.SceneManagement;

public class BlockTracker : MonoBehaviour
{
    public int[] blockArray = new int[10];  // Current pad state
    private int[] correctArrayAfterThisTurn = new int[10];

    private int turnNumber = 0;  // Current selection sort step
    public bool sortingSuccessStatus = true;

    private int initialBlocksPlaced = 0;
    private int numBlocksPlacedThisTurn = 0;

    bool firstBlockMoved = false;

    // Reference to the TextMeshPro object for displaying win/lose messages
    public TMP_Text resultText;

    void Start()
    {
        resultText.text = "";  // Ensure the text starts as blank
    }

    public void trackBlockOnPad(int blockNumber, int padIndex)
    {
        blockArray[padIndex] = blockNumber;
        Debug.Log($"Block {blockNumber} placed at pad {padIndex}");

        if (initialBlocksPlaced < 10)
        {
            initialBlocksPlaced++; 
            return; // Wait until all blocks are placed
        }

        numBlocksPlacedThisTurn++;

        if (numBlocksPlacedThisTurn == 2)
        {
            if (blockArray[turnNumber] == correctArrayAfterThisTurn[turnNumber])
            {
                Debug.Log($"Turn {turnNumber} is already correct, moving to the next turn.");
                turnNumber++;
            }
            else
            {
                ValidateSelectionSort();
                turnNumber++; // ✅ Move this here so it's also counted after successful validation
            }

            numBlocksPlacedThisTurn = 0;
            createNextCorrectArray();
            
            SkipAllCorrectTurns(); 

            CheckForWin(); // ✅ Check if the array is fully sorted
            CheckForLoss();  // ✅ Check if it's incorrect
        }
    }

    public void trackBlockOffPad(int blockNumber, int padIndex)
    {
        // Clear the block from the pad
        Debug.Log($"Block {blockNumber} removed from pad {padIndex}");

        if (!firstBlockMoved)
        {
            initializeCorrectArray();
            firstBlockMoved = true;
        }
    }

    private void ValidateSelectionSort()
    {
        for (int i = 0; i < blockArray.Length; i++)
        {
            if (blockArray[i] != correctArrayAfterThisTurn[i])
            {
                Debug.LogWarning($"Incorrect array after turn {turnNumber} at index {i}: expected {correctArrayAfterThisTurn[i]}, got {blockArray[i]}");
                sortingSuccessStatus = false;
                ShowLoseMessage();  // Show "You Lose" message
                return;
            }
        }

        Debug.Log($"Correct array after turn {turnNumber}");
        sortingSuccessStatus = true;

        // Check if the entire array is sorted and display win message
        if (turnNumber == blockArray.Length - 1)
        {
            ShowWinMessage();  // Show "You Win!" message
        }
    }

    private void createNextCorrectArray()
    {
        // Clone current blockArray
        int[] clone = new int[blockArray.Length];
        blockArray.CopyTo(clone, 0);

        // Do one Selection Sort move on the clone
        int minIndex = turnNumber;
        for (int i = turnNumber + 1; i < clone.Length; i++)
        {
            if (clone[i] < clone[minIndex])
            {
                minIndex = i;
            }
        }

        // Swap min with current turn index
        int temp = clone[turnNumber];
        clone[turnNumber] = clone[minIndex];
        clone[minIndex] = temp;

        correctArrayAfterThisTurn = clone;

        Debug.Log($"Expected array for turn {turnNumber + 1}: [{string.Join(",", correctArrayAfterThisTurn)}]");
    }

    // Call this method once at the beginning to initialize correctArrayAfterThisTurn
    public void initializeCorrectArray()
    {
        createNextCorrectArray();
        SkipAllCorrectTurns(); // Start from the first turn that actually needs a swap
    }

    // Show the "You Win!" message
    private void ShowWinMessage()
    {
        resultText.text = "You Win!";  // Update the TextMeshPro text
        resultText.color = Color.green;  // Set text color to green
    }

    // Show the "You Lose!" message
    private void ShowLoseMessage()
    {
        resultText.text = "Incorrect Move! Try Again.";  // Update the TextMeshPro text
        resultText.color = Color.red;  // Set text color to red
        
        Invoke(nameof(RestartScene), 2f);
    }
    
    private void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    
    private void CheckForWin()
    {
        for (int i = 0; i < blockArray.Length; i++)
        {
            if (blockArray[i] != i)
            {
                return; // Not sorted yet
            }
        }

        ShowWinMessage(); // Fully sorted
    }
    
    private void CheckForLoss()
    {
        for (int i = 0; i < blockArray.Length; i++)
        {
            if (blockArray[i] != correctArrayAfterThisTurn[i])
            {
                sortingSuccessStatus = false;
                ShowLoseMessage();
                return;
            }
        }
    }
    
    private void SkipAllCorrectTurns()
    {
        while (turnNumber < blockArray.Length && blockArray[turnNumber] == correctArrayAfterThisTurn[turnNumber])
        {
            Debug.Log($"Turn {turnNumber} is already correct, skipping to next turn.");
            turnNumber++;
            createNextCorrectArray(); // Update expected array for new turn
        }
    }



}
