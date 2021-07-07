using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float Speed = 5;

    void Update()
    {
        transform.Rotate(Vector3.left * Speed * Time.deltaTime, Space.Self);
        Speed += Time.deltaTime * 2.5f;
    }

}
