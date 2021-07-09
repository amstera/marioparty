using UnityEngine;

public class BumperBall : MonoBehaviour
{
    public Rigidbody Rb;
    public float Speed = 5;
    public bool IsMoving;
    public bool IsPlayer;

    void FixedUpdate()
    {
        if (IsPlayer)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                Rb.AddForce(Vector3.forward * Speed * Time.fixedDeltaTime);
                IsMoving = true;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                Rb.AddForce(Vector3.back * Speed * Time.fixedDeltaTime);
                IsMoving = true;
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                Rb.AddForce(Vector3.left * Speed * Time.fixedDeltaTime);
                IsMoving = true;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                Rb.AddForce(Vector3.right * Speed * Time.fixedDeltaTime);
                IsMoving = true;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Terrain")
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
