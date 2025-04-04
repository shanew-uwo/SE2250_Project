using UnityEngine;

public class BlockOutline : MonoBehaviour
{
    public bool isOccupied = false;  // Whether a block is placed on this BlockOutline
    public int padIndex;  // Index of the pad
    public bool previousOccupiedStatus = false;  // To track previous state of pad

    public BlockTracker blockTracker;  // Reference to the BlockTracker to update the array

    public GameObject currentBlock;  // Reference to the block placed on this BlockOutline
    public int currentBlockNumber = -1;

    void Update()
    {
        // Check if the pad's occupied status has changed
        if (isOccupied != previousOccupiedStatus)
        {
            if (isOccupied)
            {
                NumberedBlock numberedBlock = currentBlock.GetComponent<NumberedBlock>();
                currentBlockNumber = numberedBlock.GetNumber();  // Get the number from the block
                
                blockTracker.trackBlockOnPad(currentBlockNumber, padIndex);  // Add the block to the tracker
                previousOccupiedStatus = true;
            }
            else if (!isOccupied)
            {
                
                blockTracker.trackBlockOffPad(currentBlockNumber, padIndex);  // Remove the block from the tracker
                previousOccupiedStatus = false;
                
                currentBlockNumber = -1;  // Get the number from the block

            }
        }
    }
}