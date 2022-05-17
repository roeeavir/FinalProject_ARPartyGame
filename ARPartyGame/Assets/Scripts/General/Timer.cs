using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Timer : MonoBehaviour
{
    private float time = 5;
    private bool timerIsRunning = false;
    public Text timeText;

    // Start the timer and pass it a callback function
    public void StartTimer(Action callback)
    {   
        if (!timerIsRunning)
        {
            StartCoroutine(Countdown(callback));
            timerIsRunning = true;
        }
    }

    // Countdown the timer and call the callback function when the timer is finished
    private IEnumerator Countdown(Action callback)
    {
        int temp = timeText.fontSize;
        timeText.fontSize = 80;
        for (int i = 0; i < time; i++)
        {
            timeText.text = (time - i).ToString();
            FindObjectOfType<AudioManager>().Play("Timer");
            yield return new WaitForSeconds(1);
        }
        timerIsRunning = false;
        timeText.fontSize = temp;

        timeText.text = "";

        if (callback != null)
        {
            callback?.Invoke(); // Invoke the callback function
        }
    }

    // Set the time to countdown from
    public void SetTime(int t)
    {
        time = t;
    }


}