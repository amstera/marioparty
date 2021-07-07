using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterGame : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody Rb;
    public bool IsPlayer;
    public float JumpForce = 5;
    public List<GameObject> Obstacles;
    public CharacterType Type;

    private bool _isJumping;
    private bool _shouldJump;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _shouldJump = true;
        }
    }

    void FixedUpdate()
    {
        if (IsPlayer)
        {
            if (_shouldJump)
            {
                Jump();
                _shouldJump = false;
            }
        }
        else
        {
            foreach (GameObject obstacle in Obstacles)
            {
                if (Vector3.Distance(transform.position, obstacle.transform.position) < Random.Range(1.25f, 2.2f))
                {
                    Jump();
                }
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
            Rb.AddForce(collision.contacts[0].normal * 2.5f, ForceMode.Impulse);
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
        Animator.SetTrigger("Victory");
        Rb.isKinematic = true;
    }

    private void Land()
    {
        _isJumping = false;
        Animator.SetInteger("State", (int)CharacterState.Idle);
    }
}
