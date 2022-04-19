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

    public void StartTimer(Action callback)
    {   
        if (!timerIsRunning)
        {
            StartCoroutine(Countdown(callback));
            timerIsRunning = true;
        }
    }

    private IEnumerator Countdown(Action callback)
    {
        int temp = timeText.fontSize;
        timeText.fontSize = 80;
        for (int i = 0; i < time; i++)
        {
            timeText.text = (time - i).ToString();
            yield return new WaitForSeconds(1);
        }
        timerIsRunning = false;
        timeText.fontSize = temp;

        timeText.text = "";

        callback?.Invoke();
    }

    public void SetTime(int t)
    {
        time = t;
    }


}