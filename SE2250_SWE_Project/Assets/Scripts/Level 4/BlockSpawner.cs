using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BlockSpawner : MonoBehaviour
{
    public GameObject numberedBlockPrefab; // Assign your NumberedPushableBlock prefab here
    public Vector3 startPosition = Vector3.zero;
    public int numberOfBlocks = 5;
    public float spacing = 1f; // Space between blocks
    public float blockHeight = 1f;

    void Start()
    {
        SpawnShuffledBlocks();
    }

    void SpawnShuffledBlocks()
    {
        // Step 1: Create a list of numbers from 0 to 9
        List<int> numbers = new List<int>();
        for (int i = 0; i < numberOfBlocks; i++)
        {
            numbers.Add(i);
        }

        // Step 2: Shuffle the list
        for (int i = 0; i < numbers.Count; i++)
        {
            int temp = numbers[i];
            int randomIndex = Random.Range(i, numbers.Count);
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        // Step 3: Spawn a block for each number
        for (int i = 0; i < numbers.Count; i++)
        {
            Vector3 spawnPos = startPosition + new Vector3(0, blockHeight/2f, i * spacing);
            GameObject block = Instantiate(numberedBlockPrefab, spawnPos, Quaternion.identity);

            // Step 4: Assign the number to the block
            NumberedBlock nb = block.GetComponent<NumberedBlock>();
            if (nb != null)
            {
                nb.SetNumber(numbers[i]);
            }
        }
    }
}