using UnityEngine;
using UnityEngine.SceneManagement;

public class HubPortal_lvl6 : MonoBehaviour
{
    [Tooltip("Name of the scene to load when the player enters the portal.")]
    public string hubSceneName = "Level0";

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered portal. Loading hub scene: " + hubSceneName);
            SceneManager.LoadScene(hubSceneName);
        }
    }
}

