using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    public static Timer instance;
    
    public Text timer;

    private float totalTime;
    private float elapsedTime;
    private bool isRunning;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void StartTimer(float timeInSeconds)
    {
        if (!isRunning)
        {
            totalTime = timeInSeconds;
            elapsedTime = 0f;
            isRunning = true;
        }
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = Mathf.Clamp(totalTime - elapsedTime, 0f, totalTime);
            DisplayTime(remainingTime);

            if (elapsedTime >= totalTime)
            {
                StopTimer();
                EventManager.CallOnGameOver(GameStatus.RoundType == Camp.Black ? "White Win" : "Black Win");
                Debug.Log("Time's up!");
            }
        }
    }

    public void StopTimer() => isRunning = false;

    public void GoAhead() => isRunning = true;

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
        DisplayTime(totalTime);
    }

    private void DisplayTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        string timeText = $"{minutes}:{seconds}";
        timer.text = timeText;
    }
}
