using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float Speed = 5;
    public bool CanSpin;
    public AudioSource SpinAS;

    void Update()
    {
        if (!CanSpin)
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
        CanSpin = false;
    }
}