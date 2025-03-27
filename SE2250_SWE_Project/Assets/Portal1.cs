using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal1 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(1); // Load Level 1
            Debug.Log("Collision With Portal1");
        }
    }
}
