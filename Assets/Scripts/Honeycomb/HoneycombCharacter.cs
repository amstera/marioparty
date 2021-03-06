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
    public GameObject Basket;
    public GameObject Bees;

    public AudioSource VictoryAS;
    public AudioSource SadAS;
    public AudioSource StepsAS;

    private bool _shouldJump;
    private bool _isJumping;
    private bool _isWalking;
    private bool _showBasketAtDest;
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
            if (StepsAS != null && !StepsAS.isPlaying)
            {
                StepsAS.Play();
            }

            if (Mathf.Abs(transform.position.x - _walkPos.x) < Time.deltaTime * Speed || Mathf.Abs(transform.position.x - _walkPos.x) > 15)
            {
                if (StepsAS != null)
                {
                    StepsAS.Stop();
                }

                if (_showBasketAtDest)
                {
                    Basket.SetActive(true);
                }

                transform.position = new Vector3(_walkPos.x, transform.position.y, transform.position.z);
                LookAhead();
                
                Animator.SetInteger("State", Basket.activeSelf ? (int)CharacterState.Holding : (int)CharacterState.Idle);
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
            if (CanJump && _shouldJump)
            {
                Jump();
            }
        }
        else if (CanJump)
        {
            CanJump = false;
            Invoke("WaitToJump", 0.5f);
        }
        else if (_shouldJump)
        {
            var distanceToHoneycomb = HoneycombController.FruitsToHoneycomb();
            if (distanceToHoneycomb == 3)
            {
                if (YellowDice.FacingAmount == 2)
                {
                    Jump();
                }
            }
            else if (distanceToHoneycomb <= 2)
            {
                if (YellowDice.FacingAmount == 1)
                {
                    Jump();
                }
            }
            else if (YellowDice.FacingAmount != 0)
            {
                Jump();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Terrain" && _isJumping)
        {
            Land();
        }
        else if (collision.collider.name == "Dice")
        {
            YellowDice.HitSide();
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
        _shouldJump = false;
    }

    public void Win()
    {
        LookAtCamera();
        VictoryAS.Play();
        Animator.SetTrigger("Victory");
    }

    public void Lose()
    {
        Bees.SetActive(true);
        SadAS.Play();
    }

    public void WalkTowards(Vector3 pos, Action callback, bool showBasketAtDest = false)
    {
        Basket.SetActive(false);
        _showBasketAtDest = showBasketAtDest;
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

    public void LookAhead()
    {
        transform.LookAt(new Vector3(transform.position.x - 10, transform.position.y, transform.position.z));
    }

    public void LookAtCamera()
    {
        transform.LookAt(new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z));
    }

    private void Land()
    {
        if (YellowDice.CanRotate)
        {
            _isJumping = false;
            Jump();
            return;
        }

        _isJumping = false;
        CanJump = false;
        Animator.SetInteger("State", (int)CharacterState.Idle);

        WalkTowards(transform.position + Vector3.left * 3.75f, HoneycombController.MoveFruit, true);
    }

    private void WaitToJump()
    {
        _shouldJump = true;
    }
}
