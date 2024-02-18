using UnityEngine;
using UnityEngine.UI;
using Utils;


public class Timer : SingletonMono<Timer>
{
    [SerializeField]private Text timer;

    private const float TotalTime = 180.0f;
    private float _elapsedTime;
    private bool _isRunning;

    public void StartTimer()
    {
        if (!_isRunning) _isRunning = true;
    }

    private void Update()
    {
        if (_isRunning)
        {
            _elapsedTime += Time.deltaTime;
            float remainingTime = Mathf.Clamp(TotalTime - _elapsedTime, 0f, TotalTime);
            DisplayTime(remainingTime);

            if (_elapsedTime >= TotalTime)
            {
                StopTimer();
                EventManager.CallOnGameOver(GameStatus.RoundType == Camp.Black ? "White Win" : "Black Win");
                Debug.Log("Time's up!");
            }
        }
    }

    public void StopTimer() => _isRunning = false;

    public void GoAhead() => _isRunning = true;

    public void ResetTimer()
    {
        _elapsedTime = 0f;
        _isRunning = false;
        DisplayTime(TotalTime);
    }

    private void DisplayTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        string timeText = $"{minutes}:{seconds}";
        timer.text = timeText;
    }
}
