using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterGame : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody Rb;
    public bool IsPlayer;
    public bool CanJump;
    public float JumpForce = 5;
    public List<GameObject> Obstacles;
    public Spinner Spinner;
    public CharacterType Type;

    public AudioSource VictoryAS;
    public AudioSource HitAS;

    private bool _isJumping;
    private bool _shouldJump;

    void Start()
    {
        Spinner = FindObjectOfType<Spinner>();
    }

    void Update()
    {
        if (CanJump && Input.GetKeyDown(KeyCode.Space))
        {
            _shouldJump = true;
        }
    }

    void FixedUpdate()
    {
        if (!CanJump)
        {
            return;
        }

        if (IsPlayer)
        {
            if (_shouldJump)
            {
                Jump();
                _shouldJump = false;
            }
        }
        else if (!_isJumping)
        {
            var closestObstacle = Obstacles.OrderBy(o => Vector3.Distance(transform.position, o.transform.position)).First();
            float coefficient = Random.Range(33f, 36f);
            if (Vector3.Distance(transform.position, closestObstacle.transform.position) < Spinner.Speed / coefficient)
            {
                Jump();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Terrain" && _isJumping)
        {
            Land();
        }
        else if (collision.collider.name == "Spinner")
        {
            Rb.freezeRotation = false;
            Animator.SetInteger("State", (int)CharacterState.Idle);
            Vector3 normalForce = collision.contacts[0].normal;
            Rb.AddForce(new Vector3(normalForce.x, 0, normalForce.z) * 5f, ForceMode.Impulse);
            HitAS.Play();
        }
        else if (collision.collider.name == "Lava")
        {
            Destroy(gameObject);
        }
    }

    public void Jump()
    {
        if (_isJumping)
        {
            return;
        }

        Rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
        Animator.SetInteger("State", (int)CharacterState.Jump);
        _isJumping = true;
    }

    public void Win()
    {
        VictoryAS.Play();
        Physics.IgnoreCollision(GetComponent<Collider>(), FindObjectOfType<Spinner>().GetComponent<Collider>(), true);
        Animator.SetTrigger("Victory");
    }

    private void Land()
    {
        _isJumping = false;
        Animator.SetInteger("State", (int)CharacterState.Idle);
    }
}
