using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal5 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level5"); // Load Level 5
        }
    }
}