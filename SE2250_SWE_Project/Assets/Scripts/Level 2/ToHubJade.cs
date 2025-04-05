using UnityEngine;
using UnityEngine.SceneManagement;

public class ToHubJade : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(0); // Load Hub scene immediately
        }
    }
}

