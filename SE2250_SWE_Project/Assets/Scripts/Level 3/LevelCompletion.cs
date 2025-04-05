// LevelCompletion.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using TMPro; // Required for Coroutine

public class LevelCompletion : MonoBehaviour
{
    [Header("Progression Settings")]
    [SerializeField] private float timeToCompleteLevel = 60.0f;

    [Header("Dependencies")]
    [SerializeField] private RecruiterDetection recruiterDetection;
    [SerializeField] private Image progressBarImage;
    // --- NEW Dependencies for added features ---
    [Tooltip("Optional parent object of the progress bar to hide the whole group.")]
    [SerializeField] private GameObject progressBarParentObject; // Assign container if you have one
    [Tooltip("Reference to the Text component to change on completion.")]
    [SerializeField] private TextMeshProUGUI statusText; // Or change to TextMeshProUGUI if using TMP
    [Tooltip("Reference to the ChangeScene script/component.")]
    [SerializeField] private ChangeScene sceneChanger; // Assign the GameObject with ChangeScene script

    [Header("Completion Effects")]
    [Tooltip("The prefab to instantiate when the level is finished.")]
    [SerializeField] private GameObject levelCompletePrefab;
    [Tooltip("Optional: Where to spawn the completion prefab. If null, uses this object's position.")]
    [SerializeField] private Transform levelCompleteSpawnPoint;
    [Tooltip("The message to display in the Status Text on completion.")]
    [SerializeField] private string levelCompleteMessage = "LEVEL COMPLETE!";
    [Tooltip("Delay in seconds AFTER level completion before hiding UI elements.")]
    [SerializeField] private float uiHideDelay = 3.0f;

    [Header("Status (Read Only)")]
    [SerializeField] private float currentProgressTime = 0.0f;
    [SerializeField] private bool progressionPaused = false;
    [SerializeField] private bool levelFinished = false;

    [Header("Events")]
    public UnityEvent OnLevelFinished;

    // --- Variable to prevent multiple coroutine starts ---
    private Coroutine _hideUICoroutine = null;

    void Awake()
    {
        // --- Existing Awake code ---
        if (progressBarImage == null) { Debug.LogError("Progress Bar Image not assigned!", this); enabled = false; return; }
        if (progressBarImage.type != Image.Type.Filled) { progressBarImage.type = Image.Type.Filled; }
        progressBarImage.fillMethod = Image.FillMethod.Horizontal;
        progressBarImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        progressBarImage.fillAmount = 0f;

        if (recruiterDetection == null) { Debug.LogError("RecruiterDetection reference not assigned!", this); enabled = false; return; }
        if (OnLevelFinished == null) OnLevelFinished = new UnityEvent();

        // --- NEW Null Checks for added features ---
        if (statusText == null) { Debug.LogWarning("Status Text reference not assigned! Text will not change on completion.", this); } // Warning, maybe optional
        if (sceneChanger == null) { Debug.LogError("Scene Changer reference not assigned! Scene will not change.", this); enabled = false; return; } // Error, likely required
        if (levelCompletePrefab == null) { Debug.LogWarning("Level Complete Prefab not assigned! Prefab will not spawn.", this); } // Warning, maybe optional
        // progressBarParentObject and levelCompleteSpawnPoint are optional, no checks needed.
    }

    // --- THIS Update method is KEPT EXACTLY AS PROVIDED by the user ---
    void Update()
    {
        if (levelFinished) return;

        if (recruiterDetection.IsClearOfBots)
        {
            progressionPaused = false;
            if (currentProgressTime < timeToCompleteLevel)
            {
                currentProgressTime += Time.deltaTime;
                currentProgressTime = Mathf.Min(currentProgressTime, timeToCompleteLevel);

                if (currentProgressTime >= timeToCompleteLevel)
                {
                    CompleteLevel(); // Call completion logic
                }
            }
        }
        else
        {
            progressionPaused = true;
        }

        UpdateProgressBar();
    }

    // --- THIS UpdateProgressBar method is KEPT EXACTLY AS PROVIDED by the user ---
    void UpdateProgressBar()
    {
        if (progressBarImage == null) return;
        float fillValue = (timeToCompleteLevel > 0) ? currentProgressTime / timeToCompleteLevel : 0f;
        progressBarImage.fillAmount = Mathf.Clamp01(fillValue);
    }

    // --- THIS CompleteLevel method has the new features ADDED ---
    void CompleteLevel()
    {
        if (levelFinished) return; // Prevent running multiple times

        levelFinished = true; // Set flag
        currentProgressTime = timeToCompleteLevel; // Ensure time is exactly max
        UpdateProgressBar(); // Final bar update to 100%

        Debug.Log("LEVEL FINISHED! Progress reached 100%. Initiating completion sequence.");

        // --- Stop Spawners (from original logic) ---
        DisableAllEnemySpawners();

        // --- ADDED: Change Status Text ---
        if (statusText != null)
        {
            statusText.text = levelCompleteMessage;
            Debug.Log($"Status text changed to: {levelCompleteMessage}");
        }

        // --- ADDED: Instantiate Completion Prefab ---
        SpawnCompletionPrefab();

        // --- Trigger Level Finished Event (from original logic) ---
        OnLevelFinished?.Invoke();

        // --- ADDED: Start Delayed UI Hiding ---
        if (_hideUICoroutine == null) // Only start if not already running
        {
             _hideUICoroutine = StartCoroutine(HandleDelayedCompletionTasks());
        }

        // --- ADDED: Trigger the Scene Change ---
        if (sceneChanger != null)
        {
             sceneChanger.InitiateSceneChangeWithDelay(); // Let ChangeScene handle its own delay
             Debug.Log("Initiated scene change with delay.");
        }
        else {
             Debug.LogError("Cannot initiate scene change - SceneChanger reference is missing!", this);
        }
    }

    // --- ADDED: Helper method to spawn prefab ---
    void SpawnCompletionPrefab()
    {
        if (levelCompletePrefab == null) return; // Don't try if no prefab assigned

        Transform spawnTransform = (levelCompleteSpawnPoint != null) ? levelCompleteSpawnPoint : this.transform;
        Debug.Log($"Spawning Level Complete Prefab '{levelCompletePrefab.name}' at {spawnTransform.position}");
        Instantiate(levelCompletePrefab, spawnTransform.position, spawnTransform.rotation);
    }

    // --- ADDED: Coroutine to hide UI elements after a delay ---
    private IEnumerator HandleDelayedCompletionTasks()
    {
        Debug.Log($"Waiting {uiHideDelay} seconds to hide UI elements.");
        yield return new WaitForSeconds(uiHideDelay);

        Debug.Log("Hiding progress bar elements.");
        // Use the parent object if assigned, otherwise just hide the image
        GameObject objectToHide = progressBarParentObject ? progressBarParentObject : (progressBarImage ? progressBarImage.gameObject : null);
        if (objectToHide != null)
        {
            objectToHide.SetActive(false);
        }
        _hideUICoroutine = null; // Reset coroutine reference
    }


    // --- THIS DisableAllEnemySpawners method is KEPT EXACTLY AS PROVIDED by the user ---
    void DisableAllEnemySpawners()
    {
        EnemySpawner[] allSpawners = FindObjectsOfType<EnemySpawner>(); // Find all active spawners in the scene

        Debug.Log($"Found {allSpawners.Length} Enemy Spawner(s) to disable.");

        foreach (EnemySpawner spawner in allSpawners)
        {
            // Added null check for safety, but kept core logic the same
            if (spawner != null)
            {
                spawner.enabled = false; // Disable the spawner script component
                Debug.Log($"Disabled spawner: {spawner.gameObject.name}");
            }
        }
    }
}