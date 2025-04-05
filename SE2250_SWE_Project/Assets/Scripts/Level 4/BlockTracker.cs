using UnityEngine;

public class BlockTracker : MonoBehaviour
{
    public int[] blockArray = new int[10];  // Array to store the block numbers at each pad index (0-9)
    private bool[] blockMoved = new bool[10]; // To track if the block has been moved (i.e., placed in correct position)
    private int turnIndex = 0;  // Track which block (0, 1, 2, etc.) we are currently validating

    private int initialBlocksPlaced = 0;
    public void trackBlockOnPad(int blockNumber, int padIndex)
    {
        blockArray[padIndex] = blockNumber;  // Store the block number at the correct pad index
        Debug.Log($"Block {blockNumber} placed at pad {padIndex}");


        if (initialBlocksPlaced <= 10) initialBlocksPlaced++; 
        
        // once all the blocks are placed
        if (initialBlocksPlaced >= 10) ValidateSelectionSort();
        
    }

    public void trackBlockOffPad(int blockNumber, int padIndex)
    {
        blockArray[padIndex] = -1;  // Clear the block from the pad (set it to -1)
        Debug.Log($"Block {blockNumber} removed from pad {padIndex}");
    }

    private void ValidateSelectionSort()
    {
        // Validate only the current turn index block (turnIndex)
        int expectedBlock = turnIndex;
        int currentBlockAtIndex = blockArray[turnIndex];

        // If the current block at turnIndex is not the expected one, log an error
        if (currentBlockAtIndex != expectedBlock)
        {
            Debug.LogError($"Selection sort error: Block {expectedBlock} was expected at index {turnIndex}, but Block {currentBlockAtIndex} is there.");
        }

        // After each turn, move to the next block (i.e., increment turnIndex)
        if (turnIndex < blockArray.Length - 1)
        {
            turnIndex++;  // Increment the turn index to check the next block in the next step
        }
    }
}