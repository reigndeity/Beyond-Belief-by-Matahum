using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI runTimerText;
    public TextMeshProUGUI totalTimerText;

    public float runTime = 0f;
    public float totalTime = 0f;

    public bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;

        float delta = Time.deltaTime;

        runTime += delta;     // resets
        totalTime += delta;   // NEVER resets

        //UpdateUI();
    }

    void UpdateUI()
    {
        runTimerText.text = FormatTime(runTime);
        totalTimerText.text = FormatTime(totalTime);
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ResetTimer()
    {
        runTime = 0f; // ONLY resets run time
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public float GetRunTime() => runTime;
    public float GetTotalTime() => totalTime;
}
