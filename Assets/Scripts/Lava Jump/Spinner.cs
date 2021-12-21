using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float Speed = 5;
    public AudioSource SpinAS;

    private bool _canStart;

    void Start()
    {
        Invoke("StartSpinning", 2f);
    }

    void Update()
    {
        if (!_canStart)
        {
            return;
        }

        transform.Rotate(Vector3.left * Speed * Time.deltaTime, Space.Self);
        if (!SpinAS.isPlaying)
        {
            SpinAS.Play();
        }
        Speed += Time.deltaTime * 6.5f;
    }

    public void Stop()
    {
        _canStart = false;
    }

    private void StartSpinning()
    {
        _canStart = true;
    }
}