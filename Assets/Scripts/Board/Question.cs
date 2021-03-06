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

    public AudioSource SelectAS;

    private Action<bool> _callback;
    private bool _isShowingQuestion;
    private AIChoice _aiChoice;

    void Update()
    {
        if (QuestionText.text != string.Empty && _aiChoice == AIChoice.None && _isShowingQuestion)
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

    public void Show(string text, AIChoice choice, string firstAnswer = "Yes", string secondAnswer = "No", Action<bool> callback = null, float showDelay = 0)
    {
        if (_isShowingQuestion)
        {
            return;
        }

        _callback = callback;
        _aiChoice = choice;
        QuestionText.text = text;
        YesText.text = firstAnswer;
        NoText.text = secondAnswer;
        YesText.enabled = true;
        NoText.enabled = true;
        IsYes = true;
        Yes.enabled = true;

        if (showDelay > 0)
        {
            Invoke("ShowDelay", showDelay);
        }
        else
        {
            _isShowingQuestion = true;
        }

        if (_aiChoice == AIChoice.First)
        {
            Invoke("SelectChoice", 0.75f);
        }
        else if (_aiChoice == AIChoice.Second)
        {
            Invoke("ChooseNoAI", 0.5f);
        }
    }

    private void ShowDelay()
    {
        _isShowingQuestion = true;
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
        SelectAS.Play();
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
    First = 1,
    Second = 2
}
