using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal3 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //SceneManager.LoadScene(3); // Load Level 3
        }
    }
}

