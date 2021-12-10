using System.Linq;
using UnityEngine;

public class BumperBall : MonoBehaviour
{
    public Rigidbody Rb;
    public float Speed = 50;
    public GameObject WaterSplashPS;

    public AudioClip SplashClip;

    public void AddForce(Vector3 dir)
    {
        Rb.AddForce(dir * Time.fixedDeltaTime * Speed * 3f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Terrain")
        {
            AudioSource.PlayClipAtPoint(SplashClip, transform.parent.position);
            Instantiate(WaterSplashPS, transform.position, WaterSplashPS.transform.rotation);
            Destroy(transform.parent.gameObject);
        }
        else if (collision.collider.tag == "Player")
        {
            Rb.AddForce(collision.contacts.First().normal * Speed/1.25f);
        }
    }
}