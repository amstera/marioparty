using UnityEngine;

public class Character : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody Rb;
    public bool IsPlayer;
    public bool CanJump;
    public float JumpForce = 6;
    public int Roll;

    private bool _isJumping;

    private GameController _gameController;

    void Start()
    {
        _gameController = GameController.Instance;
    }

    void FixedUpdate()
    {
        if (IsPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Board" && _isJumping)
        {
            Land();
        }
    }

    public void Jump()
    {
        if (!CanJump)
        {
            return;
        }

        Animator.SetInteger("State", (int)CharacterState.Jump);
        Rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
        _isJumping = true;
        CanJump = false;
    }

    private void Land()
    {
        _isJumping = false;
        Animator.SetInteger("State", (int)CharacterState.Idle);
        _gameController.NextCharacter();
    }
}

public enum CharacterState
{
    Idle = 0,
    Jump = 1,
    Walk = 2
}