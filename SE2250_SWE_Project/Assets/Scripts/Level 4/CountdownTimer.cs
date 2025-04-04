using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 60f;
    public TMP_Text timerText;

    public BlockTracker blockTracker;
    public TMP_Text resultText;

    private bool hasTimedOut = false;

    void Update()
    {
        if (!hasTimedOut)
        {
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(0f, timeRemaining);
            UpdateTimerDisplay();

            if (timeRemaining <= 0f)
            {
                HandleTimeOut();
            }
        }
    }

    void HandleTimeOut()
    {
        hasTimedOut = true;

        if (!IsArraySorted())
        {
            resultText.text = "You ran out of time! Try again.";
            resultText.color = Color.red;
            Invoke(nameof(RestartScene), 2f);
        }
    }

    bool IsArraySorted()
    {
        for (int i = 0; i < blockTracker.blockArray.Length; i++)
        {
            if (blockTracker.blockArray[i] != i)
                return false;
        }
        return true;
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateTimerDisplay()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = seconds.ToString();

        // Change color to red when under 10 seconds
        timerText.color = seconds <= 10 ? Color.red : Color.white;
    }
}