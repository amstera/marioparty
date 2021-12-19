using System;
using UnityEngine;

public class HoneycombCharacter : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody Rb;
    public bool IsPlayer;
    public bool CanJump;
    public float JumpForce = 5;
    public float Speed = 5.5f;
    public YellowDice YellowDice; 
    public CharacterType Type;
    public HoneycombController HoneycombController;

    public AudioSource VictoryAS;
    public AudioSource SadAS;

    private bool _shouldJump;
    private bool _isJumping;
    private bool _isWalking;
    private Vector3 _walkPos;
    private Action _callback;

    void Start()
    {
        YellowDice = FindObjectOfType<YellowDice>();
        HoneycombController = FindObjectOfType<HoneycombController>();
    }

    void Update()
    {
        if (CanJump && Input.GetKeyDown(KeyCode.Space))
        {
            _shouldJump = true;
        }
        if (_isWalking)
        {
            if (Mathf.Abs(transform.position.x - _walkPos.x) < 0.2f)
            {
                transform.LookAt(new Vector3(transform.position.x - 10, transform.position.y, transform.position.z));

                Animator.SetInteger("State", (int)CharacterState.Idle);
                _isWalking = false;

                _callback?.Invoke();
            }
            else
            {
                transform.position += transform.forward * Speed * Time.deltaTime;
            }
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
        else if (CanJump)
        {
            CanJump = false;
            Invoke("Jump", 1.5f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Terrain" && _isJumping)
        {
            Land();
        }
        else if (collision.collider.name == "Side 1" || collision.collider.name == "Side 3")
        {
            YellowDice.HitSide(2);
        }
        else if (collision.collider.name == "Side 2" || collision.collider.name == "Side 4")
        {
            YellowDice.HitSide(1);
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

    public void WalkTowards(Vector3 pos, Action callback)
    {
        transform.LookAt(new Vector3(pos.x, transform.position.y, transform.position.z));
        Animator.SetInteger("State", (int)CharacterState.Walk);
        _isWalking = true;
        _walkPos = pos;
        _callback = callback;
    }

    public void MoveUp(bool isNextUp)
    {
        if (isNextUp)
        {
            WalkTowards(transform.position + Vector3.left * 1.5f, HoneycombController.StartNewTurn);
        }
        else
        {
            WalkTowards(transform.position + Vector3.left * 1.5f, null);
        }
    }

    private void Land()
    {
        _isJumping = false;
        CanJump = false;
        Animator.SetInteger("State", (int)CharacterState.Idle);

        WalkTowards(transform.position + Vector3.left * 3.75f, HoneycombController.MoveFruit);
    }
}
