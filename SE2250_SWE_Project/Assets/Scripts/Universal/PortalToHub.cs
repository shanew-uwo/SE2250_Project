using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToHub : MonoBehaviour
{
    public LevelManager levelManager; // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelManager.StopTimer();
            SceneManager.LoadScene(0); // Load Hub scene immediately
        }
    }
}