using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for IEnumerator

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private float delayInSeconds = 3.0f; // You can adjust this delay in the Inspector

    // Keep this public if you call it from a UI Button or Unity Event
    public void InitiateSceneChangeWithDelay()
    {
        // Start the coroutine that handles the delay
        StartCoroutine(LoadSceneAfterDelay());
    }

    // The Coroutine function
    private IEnumerator LoadSceneAfterDelay()
    {
        // Print a message to the console (optional, good for debugging)
        Debug.Log($"Waiting for {delayInSeconds} seconds before loading scene: {sceneName}");

        // Pause execution for the specified number of seconds
        yield return new WaitForSeconds(delayInSeconds);

        // After the delay, load the scene
        Debug.Log($"Loading scene now: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // --- Important Note ---
    // If you were previously calling a method named exactly "changeScene"
    // from a Unity Button's OnClick() event or another UnityEvent,
    // you MUST update that event in the Inspector to call
    // "InitiateSceneChangeWithDelay" instead.
}