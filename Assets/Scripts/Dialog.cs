using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public Text DialogText;
    public bool _isShowingText;
    public string _textToShow;

    private Action _callback;
    private bool _isFinished;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isFinished)
        {
            DialogText.text = string.Empty;
            _isShowingText = false;
            _textToShow = string.Empty;
            var callBackToCall = _callback;
            _callback = null;
            callBackToCall?.Invoke();
        }
    }

    public void ShowText(string text, Action callback = null)
    {
        if (_isShowingText)
        {
            return;
        }

        _callback = callback;
        _isShowingText = true;
        _textToShow = text += " ▼";
        StartCoroutine(WriteOutText());
    }

    private IEnumerator WriteOutText()
    {
        _isFinished = false;
        foreach (char c in _textToShow)
        {
            if (!_isShowingText)
            {
                yield return null;
            }
            DialogText.text += c;
            yield return new WaitForEndOfFrame();
        }

        _isFinished = true;
    }
}
