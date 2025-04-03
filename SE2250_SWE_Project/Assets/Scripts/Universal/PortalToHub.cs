using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToHub : MonoBehaviour
{
    public LevelManager levelManager; // Drag LevelManager here in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelManager.StopTimer();
            SceneManager.LoadScene(0); // Hub scene
        }
    }
}