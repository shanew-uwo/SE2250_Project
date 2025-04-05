using UnityEngine;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour
{
    public GameObject numberedBlockPrefab;  // Prefab for the block
    public Vector3 startPosition = Vector3.zero;  // Starting position for the first block
    public int numberOfBlocks = 10;            // Total number of blocks to spawn
    public float spacing = 2f;                 // Spacing between blocks
    public float blockHeight = 1f;             // Height of the blocks

    public BlockOrderTracker orderTracker;    // Reference to the BlockOrderTracker

    void Start()
    {
        SpawnShuffledBlocks();
    }

    void SpawnShuffledBlocks()
    {
        List<int> numbers = new List<int>();
        for (int i = 0; i < numberOfBlocks; i++)
        {
            numbers.Add(i);  // Fill list with numbers 0-9
        }

        // Shuffle the list to create a random order
        for (int i = 0; i < numbers.Count; i++)
        {
            int temp = numbers[i];
            int randomIndex = Random.Range(i, numbers.Count);
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        // Ensure the orderTracker.blocks array is initialized with the correct size
        //orderTracker.blocks = new GameObject[numberOfBlocks];

        // Spawn blocks and assign to the tracker
        for (int i = 0; i < numbers.Count; i++)
        {
            Vector3 spawnPos = startPosition + new Vector3(0, blockHeight / 2f, i * spacing);
            GameObject block = Instantiate(numberedBlockPrefab, spawnPos, Quaternion.identity);

            // Set the block number (e.g., 0-9)
            NumberedBlock nb = block.GetComponent<NumberedBlock>();
            if (nb != null)
            {
                nb.SetNumber(numbers[i]);
            }

            // Add block to the order tracker (stores the block in the array)
            //orderTracker.blocks[i] = block;  // Track each block
        }
    }
}