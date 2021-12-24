using System;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float TotalTime = 10;
    public bool CanStart;
    public Text TimeText;

    public AudioSource CountdownAS;

    private Action _callback;

    public void Countdown(Action callback)
    {
        _callback = callback;
        CanStart = true;

        CountdownAS.Play();
    }

    void Update()
    {
        if (!CanStart)
        {
            return;
        }

        if (TotalTime > 0)
        {
            TotalTime = Mathf.Clamp(TotalTime - Time.deltaTime, 0, TotalTime);
            if (TotalTime <= 3)
            {
                TimeText.color = Color.red;
            }

            TimeText.text = Mathf.CeilToInt(TotalTime).ToString();
        }
        else
        {
            _callback?.Invoke();
            Destroy(gameObject);
        }
    }
}
