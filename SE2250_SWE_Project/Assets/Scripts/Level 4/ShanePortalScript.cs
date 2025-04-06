using UnityEngine;
using UnityEngine.SceneManagement;

public class ShanePortalScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level0"); // Load Level 4
        }
    }
}