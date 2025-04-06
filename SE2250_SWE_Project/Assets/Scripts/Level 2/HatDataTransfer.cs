using System.Collections;
using UnityEngine;

public class HatDataTransfer : MonoBehaviour
{
    [Header("Level number for variant hats")]
    public int hatLevel = 1;

    private GameObject[] characterPrefabs;
    private LoadCharacter spawner;

    private void Start()
    {
        GameObject spawnerGO = GameObject.Find("Spawner");
        if (spawnerGO != null)
        {
            spawner = spawnerGO.GetComponent<LoadCharacter>();
            if (spawner != null)
            {
                characterPrefabs = spawner.characterPrefabs;
                Debug.Log("[HatPickup] ✅ Loaded characterPrefabs and spawner reference.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || spawner == null || characterPrefabs == null) return;

        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", -1);
        if (selectedCharacter < 0 || selectedCharacter >= characterPrefabs.Length) return;

        string baseName = characterPrefabs[selectedCharacter].name;
        string variantName = baseName + " Variant";
        string prefabPath = $"Characters/Level {hatLevel} Hats/{variantName}";

        GameObject variantPrefab = Resources.Load<GameObject>(prefabPath);
        if (variantPrefab == null)
        {
            Debug.LogError($"[HatPickup] ❌ Could NOT find prefab at path: {prefabPath}");
            return;
        }

        // Cache current position and rotation BEFORE destroying
        Vector3 spawnPosition = other.transform.position;
        Quaternion spawnRotation = other.transform.rotation;

        // Destroy the original player
        Destroy(other.gameObject);
        Debug.Log("[HatPickup] Original player destroyed.");

        // ✅ Spawn the variant at the same location using the spawner
        spawner.LoadCharacterManually(variantPrefab, spawnPosition, spawnRotation);

        // Wait for the next frame to access the newly spawned variant
        StartCoroutine(WaitForVariantAndTransferSkills(spawnPosition, spawnRotation));

        // Destroy the hat pickup object
        Destroy(gameObject); // remove hat pickup
    }

    // Coroutine to find the spawned variant after a frame
    private IEnumerator WaitForVariantAndTransferSkills(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        // Wait for the next frame to give Unity time to instantiate the object
        yield return null;

        // Now find the newly spawned variant (you can find it by position, tag, or name)
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        // Find the new player based on position
        GameObject variant = null;
        foreach (var player in allPlayers)
        {
            if (Vector3.Distance(player.transform.position, spawnPosition) < 0.1f)
            {
                variant = player;
                break;
            }
        }

        if (variant != null)
        {
            // Transfer skills or do other operations
            ManageSkills originalManageSkills = variant.GetComponent<ManageSkills>();
            // Perform the transfer (you can use your skills transfer logic here)
            Debug.Log("[HatPickup] ✅ Variant found and ready for skill transfer.");
        }
        else
        {
            Debug.LogError("[HatPickup] ❌ Variant not found.");
        }
    }
}

