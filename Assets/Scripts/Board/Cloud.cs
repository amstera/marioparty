using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float Speed = 3.5f;

    void Start()
    {
        Destroy(gameObject, 20);
    }

    void Update()
    {
        transform.position += Vector3.left * Speed * Time.deltaTime;
    }
}
