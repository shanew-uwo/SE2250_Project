using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public GameState gameState; // Drag GameStateAsset here
    public GameObject notificationPanel; // Assign Panel
    public TextMeshProUGUI notificationText; // Assign TMP Text
    public float notificationDuration = 0.75f;

    void Start()
    {
        if (gameState.showNotification)
        {
            ShowNotification(gameState.notificationMessage);
            gameState.showNotification = false;
        }
    }

    void ShowNotification(string message)
    {
        notificationPanel.SetActive(true);
        notificationText.text = message;
        StartCoroutine(HideNotification());
    }

    IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(notificationDuration);
        notificationPanel.SetActive(false);
    }
}