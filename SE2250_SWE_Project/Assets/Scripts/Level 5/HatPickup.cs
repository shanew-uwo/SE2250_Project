using UnityEngine;

public class HatPickup : MonoBehaviour
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

        Destroy(other.gameObject);
        Debug.Log("[HatPickup] Original player destroyed.");

        // ✅ Spawn variant at the same location using the spawner
        spawner.LoadCharacterManually(variantPrefab, spawnPosition, spawnRotation);

        Destroy(gameObject); // remove hat pickup
    }
}