using UnityEngine;

public class BlockLockIn : MonoBehaviour
{
    public BlockOutline targetPad;  // Reference to the BlockOutline (pad) where the block should go
    private bool isOnPad = false;
    private float timeOnPad = 0f;
    public float lockTime = 1f;  // Time required for the block to lock in place

    void Update()
    {
        // If the block is on the pad, track time for locking
        if (isOnPad && !targetPad.isOccupied)
        {
            timeOnPad += Time.deltaTime;

            // If the block stays on the pad for more than the lockTime, move it to the center
            if (timeOnPad >= lockTime)
            {
                LockBlockOnPad();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BlockOutline"))  // Ensure it only interacts with BlockOutlines
        {
            targetPad = other.GetComponent<BlockOutline>();  // Get the BlockOutline component
            isOnPad = true;
            timeOnPad = 0f;  // Reset the timer when the block enters the pad
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BlockOutline"))
        {
            // When the block leaves the pad, the pad becomes unoccupied
            targetPad.isOccupied = false;
            targetPad.currentBlock = null;
            isOnPad = false;
            timeOnPad = 0f;  // Reset the timer when block leaves the pad
        }
    }

    private void LockBlockOnPad()
    {
        if (targetPad != null && !targetPad.isOccupied)
        {
            // Move the block to the center of the pad
            SnapBlockToPadCenter();

            // Set the pad as occupied
            targetPad.isOccupied = true;

            targetPad.currentBlock = this.gameObject;
            
        }
    }

    private void SnapBlockToPadCenter()
    {
        // Move the block to the center of the pad (BlockOutline)
        Vector3 padCenter = targetPad.transform.position;  // Get the position of the pad (BlockOutline)
        padCenter.y = targetPad.transform.position.y + 0.5f;  // Set the correct height for the block (if necessary)
        transform.position = padCenter;  // Move the block to the center of the pad

        // Optionally, match the block's rotation with the pad's rotation
        transform.rotation = targetPad.transform.rotation;
    }
}
