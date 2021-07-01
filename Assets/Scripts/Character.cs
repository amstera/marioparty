using UnityEngine;

public class Character : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody Rb;
    public bool IsPlayer;
    public bool CanJump;
    public float JumpForce = 6;
    public int Roll;
    public int Coins;
    public int Stars;
    public CharacterType Type;

    public AmountDisplay AmountDisplay;

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

    public void ChangeCoins(int amount)
    {
        Coins += amount;
        AmountDisplay.Display(amount, AmountType.Coin);
        _gameController.LoadAllCharacterStats(false);
    }

    public void ChangeStars(int amount)
    {
        Stars += amount;
        AmountDisplay.Display(amount, AmountType.Star);
        _gameController.LoadAllCharacterStats(false);
    }

    private void Land()
    {
        _isJumping = false;
        Animator.SetInteger("State", (int)CharacterState.Idle);
        _gameController.CharacterLanded();
    }
}

public enum CharacterState
{
    Idle = 0,
    Jump = 1,
    Walk = 2
}

public enum CharacterType
{
    Unknown = 0,
    Mario = 1,
    Goomba = 2,
    Peach = 3,
    Boo = 4
}