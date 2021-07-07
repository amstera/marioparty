using System.Collections.Generic;
using UnityEngine;

public class CharacterGame : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody Rb;
    public bool IsPlayer;
    public float JumpForce = 5;
    public List<GameObject> Obstacles;

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
                if (Vector3.Distance(transform.position, obstacle.transform.position) < Random.Range(1.4f, 2f))
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

    private void Land()
    {
        _isJumping = false;
        Animator.SetInteger("State", (int)CharacterState.Idle);
    }
}
