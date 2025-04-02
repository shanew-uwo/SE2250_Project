using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToHub : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportToHub();
        }
    }

    void TeleportToHub()
    {
        SceneManager.LoadScene(0); // Scene 0 = Hub
        Debug.Log("Player teleported back to Hub.");
    }
}