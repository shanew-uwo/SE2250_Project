using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal6 : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Scenes/Level 6 scenes/Level6PreBossfight");  //Load Level 6
        }
    }
}