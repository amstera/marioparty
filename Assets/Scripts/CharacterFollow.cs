using UnityEngine;

public class CharacterFollow : MonoBehaviour
{
    public Animator Animator;
    public BumperBall Ball;
    public GameObject Character;
    public bool IsPlayer;
    public bool IsMoving;

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
        if (!_startedMoving && IsMoving)
        {
            Animator.SetInteger("State", (int)CharacterState.Walk);
            _startedMoving = true;
        }

        _forceToAdd = Vector3.zero;

        var remainingCharacters = FindObjectsOfType<CharacterFollow>();
        if (remainingCharacters.Length == 1)
        {
            Ball.Rb.isKinematic = true;
        }

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
                if (distance < closestDistance)
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
                var diff = (closestBumperBall.transform.position - Ball.transform.position).normalized;
                _forceToAdd = diff;
            }
        }
    }

    void LateUpdate()
    {
        Character.transform.position = Ball.transform.position + _characterOffset;
        if (_targetBall != null)
        {
            Character.transform.LookAt(new Vector3(_targetBall.transform.position.x, Character.transform.position.y, _targetBall.transform.position.z));
        }
        else if (IsPlayer)
        {
            Character.transform.LookAt(new Vector3(_forceToAdd.x * 10, Character.transform.position.y, _forceToAdd.z * 10));
        }
    }

    private void FixedUpdate()
    {
        if (_forceToAdd != Vector3.zero)
        {
            Ball.AddForce(_forceToAdd);
        }
    }
}
