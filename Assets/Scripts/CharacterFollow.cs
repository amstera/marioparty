using UnityEngine;

public class CharacterFollow : MonoBehaviour
{
    public Animator Animator;
    public BumperBall Ball;
    public float Speed = 50;
    public bool IsRotated;

    private Vector3 _offset;
    private bool _startedMoving;
    private int _multiplier;

    void Start()
    {
        _offset = transform.position - Ball.transform.position;
        _multiplier = IsRotated ? -1 : 1;
    }

    void Update()
    {
        if (!_startedMoving && Ball.IsMoving)
        {
            Animator.SetInteger("State", (int)CharacterState.Walk);
            _startedMoving = true;
        }

        if (Ball.IsPlayer)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(0, -Speed * _multiplier, 0) * Time.deltaTime, Space.Self);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(0, Speed * _multiplier, 0) * Time.deltaTime, Space.Self);
            }
        }
    }

    void LateUpdate()
    {
        if (Ball != null)
        {
            transform.position = Ball.transform.position + _offset;
        }
    }
}
