using System.Linq;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float Speed = 5;

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
        Speed += Time.deltaTime * 2.75f;
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