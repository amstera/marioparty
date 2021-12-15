using System;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public float RaiseSpeed = 1;

    public AudioSource PipeSoundAS;

    private bool _goingUp;
    private Action _callback;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, -1.2f, transform.position.z);
    }

    void Update()
    {
        if (_goingUp && transform.position.y < 0.6)
        {
            transform.position += Vector3.up * RaiseSpeed * Time.deltaTime;

            if (transform.position.y >= 0.6)
            {
                _callback?.Invoke();
                _goingUp = false;
            }
        }

        if (!_goingUp)
        {
            transform.position += Vector3.down * RaiseSpeed * Time.deltaTime;

            if (transform.position.y <= -1.2)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Show(Action callback)
    {
        _goingUp = true;
        _callback = callback;

        PipeSoundAS.Play();
    }
}
