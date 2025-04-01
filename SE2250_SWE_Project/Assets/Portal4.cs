using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal4 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //SceneManager.LoadScene(4); // Load Level 4
        }
    }
}

