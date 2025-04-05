using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BlockTracker : MonoBehaviour
{
    public int[] blockArray = new int[10];
    private int[] correctArrayAfterThisTurn = new int[10];

    private int turnNumber = 0;
    public bool sortingSuccessStatus = true;

    private int initialBlocksPlaced = 0;
    private int numBlocksPlacedThisTurn = 0;

    public TMP_Text resultText;

    void Start()
    {
        resultText.text = "";
    }

    public void trackBlockOnPad(int blockNumber, int padIndex)
    {
        blockArray[padIndex] = blockNumber;
        Debug.Log($"Block {blockNumber} placed at pad {padIndex}");

        if (initialBlocksPlaced < 10)
        {
            initialBlocksPlaced++;

            if (initialBlocksPlaced == 10)
            {
                initializeCorrectArray();
            }

            return;
        }

        numBlocksPlacedThisTurn++;

        if (numBlocksPlacedThisTurn == 2)
        {
            Debug.Log($"BLOCK ARRAY:            [{string.Join(",", blockArray)}]");
            Debug.Log($"EXPECTED ARRAY (TURN {turnNumber}): [{string.Join(",", correctArrayAfterThisTurn)}]");

            ValidateSelectionSort();

            numBlocksPlacedThisTurn = 0;

            turnNumber++;               // ✅ First increment
            createNextCorrectArray();   // ✅ Now create array for the new turn

            SkipAllCorrectTurns();
            CheckForWin();
        }
    }

    public void trackBlockOffPad(int blockNumber, int padIndex)
    {
        Debug.Log($"Block {blockNumber} removed from pad {padIndex}");
    }

    private void ValidateSelectionSort()
    {
        for (int i = 0; i < blockArray.Length; i++)
        {
            if (blockArray[i] != correctArrayAfterThisTurn[i])
            {
                Debug.LogWarning($"Incorrect at index {i}: expected {correctArrayAfterThisTurn[i]}, got {blockArray[i]}");
                sortingSuccessStatus = false;
                ShowLoseMessage();
                return;
            }
        }

        Debug.Log($"Turn {turnNumber} validated: Correct swap!");
        sortingSuccessStatus = true;

        if (turnNumber == blockArray.Length - 1)
        {
            ShowWinMessage();
        }
    }

    private void createNextCorrectArray()
    {
        int[] clone = new int[blockArray.Length];
        blockArray.CopyTo(clone, 0);

        int minIndex = turnNumber;
        for (int i = turnNumber + 1; i < clone.Length; i++)
        {
            if (clone[i] < clone[minIndex])
            {
                minIndex = i;
            }
        }

        int temp = clone[turnNumber];
        clone[turnNumber] = clone[minIndex];
        clone[minIndex] = temp;

        correctArrayAfterThisTurn = clone;

        Debug.Log($"Generated expected array for turn {turnNumber}: [{string.Join(",", correctArrayAfterThisTurn)}]");
    }

    public void initializeCorrectArray()
    {
        createNextCorrectArray();
        SkipAllCorrectTurns();
    }

    private void ShowWinMessage()
    {
        resultText.text = "You Win!";
        resultText.color = Color.green;
        
        // ✅ Tell the countdown timer to stop
        FindObjectOfType<CountdownTimer>().gameWon = true;
        
        SceneManager.LoadScene("Level0"); 
    }

    private void ShowLoseMessage()
    {
        resultText.text = "Incorrect Move! Try Again.";
        resultText.color = Color.red;

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
                return;
        }

        ShowWinMessage();
    }

    private void SkipAllCorrectTurns()
    {
        while (turnNumber < blockArray.Length)
        {
            // Generate what the correct array would look like at this turn
            int[] clone = new int[blockArray.Length];
            blockArray.CopyTo(clone, 0);

            int minIndex = turnNumber;
            for (int i = turnNumber + 1; i < clone.Length; i++)
            {
                if (clone[i] < clone[minIndex])
                {
                    minIndex = i;
                }
            }

            int temp = clone[turnNumber];
            clone[turnNumber] = clone[minIndex];
            clone[minIndex] = temp;

            // Check if the player's current array already matches this "correct after turn" state
            bool alreadyCorrect = true;
            for (int i = 0; i < clone.Length; i++)
            {
                if (blockArray[i] != clone[i])
                {
                    alreadyCorrect = false;
                    break;
                }
            }

            if (alreadyCorrect)
            {
                Debug.Log($"Turn {turnNumber} already correct. Skipping.");
                correctArrayAfterThisTurn = clone;
                turnNumber++;
            }
            else
            {
                correctArrayAfterThisTurn = clone; // Still update to expected even if not skipping
                break;
            }
        }
    }
}
