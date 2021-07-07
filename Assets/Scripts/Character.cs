using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody Rb;
    public bool IsPlayer;
    public bool CanJump;
    public float JumpForce = 6;
    public float Speed = 5;
    public int Roll;
    public int Position;
    public int Coins;
    public int Stars;
    public CharacterType Type;
    public Queue<Circle> Destinations;

    public AmountDisplay AmountDisplay;

    private bool _isJumping;
    private bool _isWalking;

    private GameController _gameController;

    void Start()
    {
        _gameController = GameController.Instance;
        AmountDisplay.transform.SetParent(null);
        foreach (Character character in FindObjectsOfType<Character>())
        {
            if (character.Type == Type)
            {
                continue;
            }
            Physics.IgnoreCollision(GetComponent<Collider>(), character.GetComponent<Collider>(), true);
        }
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

    void Update()
    {
        if (_isWalking)
        {
            Walk();
        }
        else
        {
            transform.LookAt(new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Terrain" && _isJumping)
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

    public void WalkTowards(Queue<Circle> destinations)
    {
        _isWalking = true;
        Animator.SetInteger("State", (int)CharacterState.Walk);
        Destinations = destinations;
    }

    public void ChangeCoins(int amount)
    {
        Coins = Mathf.Clamp(Coins + amount, 0, 100);
        AmountDisplay.Display(amount, AmountType.Coin, transform.position);
        if (amount > 0)
        {
            Animator.SetTrigger("Victory");
        }
        _gameController.LoadAllCharacterStats(false);
    }

    public void ChangeStars(int amount)
    {
        Stars += Mathf.Clamp(Stars + amount, 0, 100);
        AmountDisplay.Display(amount, AmountType.Star, transform.position);
        if (amount > 0)
        {
            Animator.SetTrigger("Victory");
        }
        _gameController.LoadAllCharacterStats(false);
    }

    private void Walk()
    {
        Circle space = Destinations.Peek();
        Vector3 pos = PositionFromSpace(space);
        if (Vector3.Distance(transform.position, pos) < 0.2f)
        {
            Destinations.Dequeue();
            if (Destinations.Count == 0 || space.Type == CircleType.Star)
            {
                Animator.SetInteger("State", (int)CharacterState.Idle);
                _isWalking = false;
                _gameController.ReachedSpace(this, space);
                return;
            }
            pos = PositionFromSpace(Destinations.Peek());
        }

        transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    private void Land()
    {
        _isJumping = false;
        Animator.SetInteger("State", (int)CharacterState.Idle);
        _gameController.CharacterLanded(this);
    }

    private Vector3 PositionFromSpace(Circle space)
    {
        Vector3 spacePosition = space.transform.position;
        var pos = new Vector3(spacePosition.x, transform.position.y, spacePosition.z);
        return pos;
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