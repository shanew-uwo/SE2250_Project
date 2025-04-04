using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Custom/Game State")]
public class GameState : ScriptableObject
{
    public bool showNotification;
    public string notificationMessage;
}