using System;
using UnityEngine;

public class EventText : MonoBehaviour
{
    private Action _callback;

    public void Show(Action callback)
    {
        _callback = callback;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _callback?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
