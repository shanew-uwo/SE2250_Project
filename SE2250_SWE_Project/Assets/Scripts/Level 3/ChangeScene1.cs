using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeScene1 : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private float delayInSeconds = 3.0f;

    // Public method to START the delayed loading process
    public void InitiateSceneChangeWithDelay()
    {
        // Prevent starting multiple load routines if already called
        StopAllCoroutines();
        StartCoroutine(LoadSceneAfterDelay());
    }

    // Public method to load IMMEDIATELY (useful if delay handled elsewhere)
    public void LoadSceneDirectly()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene Name is not set in ChangeScene script!", this);
            return;
        }
        Debug.Log($"Loading scene directly: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }


    // The Coroutine function for delayed loading
    private IEnumerator LoadSceneAfterDelay()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene Name is not set in ChangeScene script! Cannot load after delay.", this);
            yield break; // Stop the coroutine
        }

        Debug.Log($"Waiting for {delayInSeconds} seconds before loading scene: {sceneName}");
        yield return new WaitForSeconds(delayInSeconds);

        Debug.Log($"Loading scene now: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}