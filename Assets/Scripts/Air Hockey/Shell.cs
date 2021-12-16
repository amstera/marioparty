using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody Rb;
    public float Force = 6.5f;

    private AirHockeyController _airHockeyController;

    void Start()
    {
        _airHockeyController = FindObjectOfType<AirHockeyController>();

        Vector3 multiplier;
        int randomNumber = Random.Range(0, 4);
        switch (randomNumber)
        {
            case 1:
                multiplier = new Vector3(1, 0, -1);
                break;
            case 2:
                multiplier = new Vector3(-1, 0, 1);
                break;
            case 3:
                multiplier = new Vector3(1, 0, 1);
                break;
            default:
                multiplier = new Vector3(-1, 0, -1);
                break;
        }

        Rb.AddForce(multiplier * Force, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Blue Net")
        {
            _airHockeyController.AddScore(TeamColor.Red);
            Destroy(gameObject);
        }
        else if (collision.collider.name == "Red Net")
        {
            _airHockeyController.AddScore(TeamColor.Blue);
            Destroy(gameObject);
        }
    }
}