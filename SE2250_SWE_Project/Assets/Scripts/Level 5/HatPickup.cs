using UnityEngine;

public class HatPickup : MonoBehaviour
{
    [Header("Level number for variant hats")]
    public int hatLevel = 1;

    // Cache references to avoid repeated Find calls
    private LoadCharacter spawner;
    private GameObject[] characterPrefabs;

    private void Start()
    {
        // Cache spawner reference
        GameObject spawnerGO = GameObject.Find("Spawner"); // Ensure your Spawner object is named "Spawner"
        if (spawnerGO != null)
        {
            spawner = spawnerGO.GetComponent<LoadCharacter>();
            if (spawner != null)
            {
                // Get prefabs from the cached spawner reference
                characterPrefabs = spawner.characterPrefabs;
                Debug.Log("[HatPickup] ✅ Cached spawner and characterPrefabs reference.");
            }
            else
            {
                Debug.LogError("[HatPickup] ❌ Spawner GameObject found, but missing LoadCharacter component.");
            }
        }
        else
        {
            Debug.LogError("[HatPickup] ❌ Could not find GameObject named 'Spawner'.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for valid player, spawner, and prefabs early
        if (!other.CompareTag("Player")) return; // Only interact with player
        if (spawner == null) {
             Debug.LogWarning("[HatPickup] Spawner reference is null. Cannot proceed.");
             return;
        }
         if (characterPrefabs == null || characterPrefabs.Length == 0) {
             Debug.LogWarning("[HatPickup] Character prefabs not loaded. Cannot proceed.");
             return;
        }


        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", -1); // Use default -1 to check if set
        if (selectedCharacter < 0 || selectedCharacter >= characterPrefabs.Length)
        {
            Debug.LogError($"[HatPickup] ❌ Invalid selectedCharacter index ({selectedCharacter}). Cannot find base prefab.");
            return;
        }

        // Ensure the selected character prefab itself isn't null
        if (characterPrefabs[selectedCharacter] == null) {
            Debug.LogError($"[HatPickup] ❌ Character prefab at index {selectedCharacter} is null.");
            return;
        }

        string baseName = characterPrefabs[selectedCharacter].name;
        string variantName = baseName + " Variant"; // Make sure your prefab naming convention matches exactly
        // Consider making the " Variant" suffix a configurable field if needed
        string prefabPath = $"Characters/Level {hatLevel} Hats/{variantName}";

        Debug.Log($"[HatPickup] Attempting to load variant prefab at path: {prefabPath}");
        GameObject variantPrefab = Resources.Load<GameObject>(prefabPath);

        if (variantPrefab == null)
        {
            // Provide more context for the error
            Debug.LogError($"[HatPickup] ❌ Could NOT load prefab at path: '{prefabPath}'. " +
                           $"Check if the path is correct, the prefab exists, and it's in a Resources folder.");
            return;
        }

        // Cache current position and rotation BEFORE destroying the original player
        Vector3 spawnPosition = other.transform.position;
        Quaternion spawnRotation = other.transform.rotation;

        // It's generally safer to disable the original player first, spawn the new one,
        // then destroy the original. This avoids a frame where there might be no player.
        GameObject originalPlayer = other.gameObject;
        originalPlayer.SetActive(false); // Temporarily disable


        // Spawn variant at the same location using the spawner
        // LoadCharacterManually will now also trigger the configuration
        spawner.LoadCharacterManually(variantPrefab, spawnPosition, spawnRotation);
        Debug.Log($"[HatPickup] Spawned variant '{variantPrefab.name}'.");


        // Now destroy the original player and the pickup
        Destroy(originalPlayer);
        Debug.Log("[HatPickup] Original player destroyed.");
        Destroy(gameObject);
        Debug.Log("[HatPickup] Hat pickup destroyed.");
    }
}