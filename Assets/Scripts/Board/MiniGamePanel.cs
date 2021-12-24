using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamePanel : MonoBehaviour
{
    public List<MiniGameOption> MiniGames;
    public CameraMove Camera;

    private Action _callback;
    private int _chosenIndex;
    private int _showingIndex;
    private int _rotatingSelectionCount;

    public AudioSource SelectionAS;

    public void Show(int index, Action callback)
    {
        gameObject.SetActive(true);
        Camera.Blur.enabled = true;

        _callback = callback;
        _chosenIndex = index;
        _rotatingSelectionCount = 0;
        _showingIndex = 0;

        Invoke("DoRotatingSelection", 0.25f);
    }

    private void DoRotatingSelection()
    {
        MiniGames[_showingIndex].Select(false);
        _showingIndex = (_showingIndex + 1) % MiniGames.Count;
        MiniGames[_showingIndex].Select(true);

        _rotatingSelectionCount++;

        if (_rotatingSelectionCount > 12 && _showingIndex == _chosenIndex)
        {
            SelectionAS.pitch = 1.25f;
            SelectionAS.Play();

            Invoke("CallCallback", 1f);
        }
        else
        {
            SelectionAS.pitch = 1f;
            SelectionAS.Play();

            Invoke("DoRotatingSelection", _rotatingSelectionCount < 10 ? 0.125f : 0.35f);
        }
    }

    private void CallCallback()
    {
        _callback?.Invoke();
    }
}
