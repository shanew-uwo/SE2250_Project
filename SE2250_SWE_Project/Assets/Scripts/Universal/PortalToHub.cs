using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToHub : MonoBehaviour
{
    public LevelManager levelManager; // Assign in Inspector

    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision");
        if (other.CompareTag("Player"))
        {
            Debug.Log("player");
            levelManager.StopTimer();
            SceneManager.LoadScene(1); // Load Hub scene immediately
        }
    }
}