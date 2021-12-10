using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraMove : MonoBehaviour
{
    public Vector3 StartOffset;
    public Character Target;
    public BlurOptimized Blur;
    public Vector3 BoardEnd;

    private Vector3 _offset;
    private GameController _gameController;
    private bool _panBoard;
    private bool _reachedBoardEnd;
    private Action _callback;

    void Start()
    {
        _offset = transform.position - StartOffset;
        _gameController = GameController.Instance;
    }

    void LateUpdate()
    {
        if (_panBoard)
        {
            if (_reachedBoardEnd)
            {
                Vector3 pos = StartOffset + _offset;
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 1.5f);
                if (Vector3.Distance(transform.position, pos) < 0.5f)
                {
                    _panBoard = false;
                    _callback?.Invoke();
                }
            }
            else
            {
                Vector3 pos = BoardEnd + _offset;
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 1.5f);
                if (Vector3.Distance(transform.position, pos) < 0.5f && !_reachedBoardEnd)
                {
                    _reachedBoardEnd = true;
                }
            }
        }
        else
        {
            Target = _gameController?.GetCurrentCharacter() ?? null;
            if (Target != null)
            {
                var newPos = Target.transform.position + _offset;
                if (Target.Position >= 22 && Target.Position <= 32)
                {
                    newPos += Vector3.up * 0.5f;
                    newPos += Vector3.forward * 2;
                }
                transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 2.5f);
            }
        }
    }

    public void PanBoard(Action callback)
    {
        Invoke("StartPan", 0.5f);
        _callback = callback;
    }

    private void StartPan()
    {
        _panBoard = true;
    }
}