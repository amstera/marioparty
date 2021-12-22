using UnityEngine;

public class Fish : MonoBehaviour
{
    public float Speed = 5;

    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Terrain" && Time.timeSinceLevelLoad > 1)
        {
            FlipDirection();
        }
    }

    private void FlipDirection()
    {
        transform.Rotate(0, 180, 0, Space.Self);
    }
}
