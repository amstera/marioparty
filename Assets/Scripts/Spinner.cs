using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float Speed = 5;

    void Update()
    {
        transform.Rotate(Vector3.left * Speed * Time.deltaTime, Space.Self);
    }
}
