using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public Text DialogText;
    public bool _isShowingText;
    public string _textToShow;

    public AudioSource SelectAS;

    private Action _callback;
    private bool _cpuApprove;
    private float _timeTextShown;

    void Update()
    {
        if ((_cpuApprove ? Time.time - _timeTextShown > 0.5f : Input.GetKeyDown(KeyCode.Space)) && DialogText.text == _textToShow && _isShowingText)
        {
            Hide(true);
        }
    }

    public void ShowText(string text, bool cpuApprove, Action callback = null)
    {
        if (_isShowingText)
        {
            return;
        }

        _callback = callback;
        _isShowingText = true;
        _textToShow = text += " ▼";
        _cpuApprove = cpuApprove;
        StartCoroutine(WriteOutText());
    }

    public void Hide(bool usedEntered)
    {
        if (usedEntered)
        {
            SelectAS.Play();
        }
        else
        {
            _callback = null;
        }
        DialogText.text = string.Empty;
        _isShowingText = false;
        _textToShow = string.Empty;
        var callBackToCall = _callback;
        _callback = null;
        callBackToCall?.Invoke();
    }

    private IEnumerator WriteOutText()
    {
        foreach (char c in _textToShow)
        {
            if (!_isShowingText)
            {
                yield return null;
            }
            DialogText.text += c;
            if (DialogText.text == _textToShow)
            {
                _timeTextShown = Time.time;
                yield return null;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
