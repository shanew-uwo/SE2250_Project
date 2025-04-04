// LevelCompletion.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LevelCompletion : MonoBehaviour
{
    [Header("Progression Settings")]
    [SerializeField] private float timeToCompleteLevel = 60.0f;

    [Header("Dependencies")]
    [SerializeField] private RecruiterDetection recruiterDetection;
    [SerializeField] private Image progressBarImage;

    [Header("Status (Read Only)")]
    [SerializeField] private float currentProgressTime = 0.0f;
    [SerializeField] private bool progressionPaused = false;
    [SerializeField] private bool levelFinished = false;

    [Header("Events")]
    public UnityEvent OnLevelFinished;

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
    }

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

    void UpdateProgressBar()
    {
        if (progressBarImage == null) return;
        float fillValue = (timeToCompleteLevel > 0) ? currentProgressTime / timeToCompleteLevel : 0f;
        progressBarImage.fillAmount = Mathf.Clamp01(fillValue);
    }

    void CompleteLevel()
    {
        if (levelFinished) return;

        levelFinished = true;
        currentProgressTime = timeToCompleteLevel;
        UpdateProgressBar(); // Final bar update

        Debug.Log("LEVEL FINISHED! Progress reached 100%. Disabling Spawners.");
        OnLevelFinished?.Invoke(); // Trigger completion event

        // --- Call the new function to stop spawners ---
        DisableAllEnemySpawners();

        // Add other level end logic here (load next scene, show UI, etc.)
    }

    // --- New Function to Stop Spawners ---
    void DisableAllEnemySpawners()
    {
        EnemySpawner[] allSpawners = FindObjectsOfType<EnemySpawner>(); // Find all active spawners in the scene

        Debug.Log($"Found {allSpawners.Length} Enemy Spawner(s) to disable.");

        foreach (EnemySpawner spawner in allSpawners)
        {
            spawner.enabled = false; // Disable the spawner script component
            // Alternatively, you could disable the whole GameObject:
            // spawner.gameObject.SetActive(false);
            Debug.Log($"Disabled spawner: {spawner.gameObject.name}");
        }
    }
}