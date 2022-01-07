using System;
using UnityEngine;

public class EventText : MonoBehaviour
{
    private Action _callback;
    private bool _isPlayer;

    public void Show(Action callback, bool isPlayer)
    {
        _callback = callback;
        _isPlayer = isPlayer;
        gameObject.SetActive(true);

        if (!_isPlayer)
        {
            Invoke("Exit", 1.25f);
        }
    }

    private void Update()
    {
        if (_isPlayer && Input.GetKeyDown(KeyCode.Space))
        {
            Exit();
        }
    }

    private void Exit()
    {
        _callback?.Invoke();
        gameObject.SetActive(false);
    }
}
