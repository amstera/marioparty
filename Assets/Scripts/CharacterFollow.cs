using UnityEngine;

public class CharacterFollow : MonoBehaviour
{
    public Animator Animator;
    public BumperBall Ball;
    public GameObject Character;
    public CharacterType CharacterType;
    public bool IsPlayer;
    public bool IsMoving;
    public bool CanMove;

    private Vector3 _characterOffset;
    private bool _startedMoving;
    private Vector3 _forceToAdd = Vector3.zero;
    private BumperBall _targetBall;

    void Start()
    {
        _characterOffset = Character.transform.position - Ball.transform.position;
    }

    void Update()
    {
        if (!CanMove)
        {
            return;
        }

        if (!_startedMoving && IsMoving)
        {
            Animator.SetInteger("State", (int)CharacterState.Walk);
            _startedMoving = true;
        }

        _forceToAdd = Vector3.zero;

        if (IsPlayer)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                _forceToAdd = Vector3.forward;
                IsMoving = true;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                _forceToAdd = Vector3.back;
                IsMoving = true;
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _forceToAdd += Vector3.left;
                IsMoving = true;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _forceToAdd += Vector3.right;
                IsMoving = true;
            }
        }
        else
        {
            var bumperBalls = FindObjectsOfType<BumperBall>();
            BumperBall closestBumperBall = null;
            float closestDistance = Mathf.Infinity;
            foreach (BumperBall bumperBall in bumperBalls)
            {
                if (bumperBall == Ball)
                {
                    continue;
                }
                var distance = Vector3.Distance(transform.position, bumperBall.transform.position);
                if (distance < closestDistance && bumperBall.transform.position.y > 0)
                {
                    closestDistance = distance;
                    closestBumperBall = bumperBall;
                    _targetBall = closestBumperBall;
                }
            }

            if (closestBumperBall == null)
            {
                _forceToAdd = Vector3.zero;
            }
            else
            {
                IsMoving = true;
                var diff = (closestBumperBall.transform.position - Ball.transform.position).normalized;
                _forceToAdd = diff;
            }
        }
    }

    void LateUpdate()
    {
        Character.transform.position = Ball.transform.position + _characterOffset;
        Vector3 lookAt = new Vector3(_forceToAdd.x * 10, Character.transform.position.y, _forceToAdd.z * 10);
        if (_targetBall != null)
        {
            lookAt = new Vector3(_targetBall.transform.position.x, Character.transform.position.y, _targetBall.transform.position.z);
        }

        var targetRotation = Quaternion.LookRotation(lookAt - Character.transform.position);
        Character.transform.rotation = Quaternion.Slerp(Character.transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (_forceToAdd != Vector3.zero)
        {
            Ball.AddForce(_forceToAdd);
        }
    }

    public void Win()
    {
        Ball.Rb.isKinematic = true;
        CanMove = false;
        Animator.SetInteger("State", (int)CharacterState.Idle);
        Animator.SetTrigger("Victory");
    }
}
