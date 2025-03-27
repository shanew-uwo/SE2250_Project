using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal6 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //SceneManager.LoadScene(6); // Load Level 6
        }
    }
}