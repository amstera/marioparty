using System;
using UnityEngine;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
    public Text QuestionText;
    public Text YesText;
    public Text NoText;
    public RawImage Yes;
    public RawImage No;
    public bool IsYes = true;

    private Action<bool> _callback;
    private bool _isShowingQuestion;
    private AIChoice _aiChoice;

    void Update()
    {
        if (QuestionText.text != string.Empty && _aiChoice == AIChoice.None)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (IsYes)
                {
                    ChooseNo();
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (!IsYes)
                {
                    ChooseYes();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                SelectChoice();
            }
        }
    }

    public void Show(string text, AIChoice choice, Action<bool> callback = null)
    {
        if (_isShowingQuestion)
        {
            return;
        }

        _callback = callback;
        _isShowingQuestion = true;
        _aiChoice = choice;
        QuestionText.text = text;
        YesText.enabled = true;
        NoText.enabled = true;
        IsYes = true;
        Yes.enabled = true;

        if (_aiChoice == AIChoice.Yes)
        {
            Invoke("SelectChoice", 0.75f);
        }
        else if (_aiChoice == AIChoice.No)
        {
            Invoke("ChooseNoAI", 0.5f);
        }
    }

    private void ChooseNo()
    {
        IsYes = false;
        Yes.enabled = false;
        No.enabled = true;
    }

    private void ChooseYes()
    {
        IsYes = true;
        Yes.enabled = true;
        No.enabled = false;
    }

    private void SelectChoice()
    {
        QuestionText.text = string.Empty;
        _isShowingQuestion = false;
        Yes.enabled = false;
        No.enabled = false;
        YesText.enabled = false;
        NoText.enabled = false;
        var callBackToCall = _callback;
        _callback = null;
        callBackToCall?.Invoke(IsYes);
    }

    private void ChooseNoAI()
    {
        ChooseNo();
        Invoke("SelectChoice", 0.25f);
    }
}

public enum AIChoice
{
    None = 0,
    Yes = 1,
    No = 2
}
