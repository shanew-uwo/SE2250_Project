using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal4 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level4"); // Load Level 4
        }
    }
}

