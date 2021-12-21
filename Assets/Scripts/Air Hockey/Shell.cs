using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody Rb;
    public float Force = 6.5f;

    public AudioSource ShellHitAS;

    private AirHockeyController _airHockeyController;
    private bool _hasHitGround;
    private float _timeFrozen;

    void Start()
    {
        _airHockeyController = FindObjectOfType<AirHockeyController>();
        Invoke("AddStartingForce", 0.5f);
    }

    void FixedUpdate()
    {
        if (Rb.velocity.magnitude < 0.5f)
        {
            _timeFrozen += Time.fixedDeltaTime;
            if (_timeFrozen > 5)
            {
                AddStartingForce();
                _timeFrozen = 0;
            }
        }
        else
        {
            _timeFrozen = 0;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Inside Blue")
        {
            _airHockeyController.AddScore(TeamColor.Red);
            Destroy(gameObject);
        }
        else if (collision.collider.name == "Inside Red")
        {
            _airHockeyController.AddScore(TeamColor.Blue);
            Destroy(gameObject);
        }
        else if (collision.collider.name != "Plane")
        {
            ShellHitAS.Play();
        }
    }

    private void AddStartingForce()
    {
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
}