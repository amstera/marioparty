using UnityEngine;

public class FlyGuy : MonoBehaviour
{
    public Animator Animator;

    public bool CanMash;
    public bool CanMove;
    public bool IsPlayer;
    public bool IsFinished;
    public float DistanceSpun;
    public float Speed = 1.5f;
    public CharacterType CharacterType;
    public GameObject Gear;

    public AudioSource HelicopterAS;

    private MashMarathonController _mashMarathonController;
    private bool _isSecondKey;

    void Start()
    {
        _mashMarathonController = FindObjectOfType<MashMarathonController>();
    }

    void Update()
    {
        if (CanMash)
        {
            if (IsPlayer)
            {
                if (_isSecondKey && Input.GetKeyDown(KeyCode.D))
                {
                    DistanceSpun += 0.5f;
                    _isSecondKey = false;
                    RotateGear(true, true);
                }
                else if (!_isSecondKey && Input.GetKeyDown(KeyCode.A))
                {
                    _isSecondKey = true;
                    RotateGear(true, true);
                }
            }
            else
            {
                RotateGear(true, false);
            }
        }
        else if (CanMove && DistanceSpun > 0)
        {
            if (transform.localPosition.y < 0.6f)
            {
                transform.position += Vector3.up * Speed * Time.deltaTime;
            }
            transform.position += transform.forward * Speed * Time.deltaTime;
            DistanceSpun -= Time.deltaTime;
            RotateGear(false, false);
        }
        else if (CanMove && DistanceSpun <= 0)
        {
            CanMove = false;
            IsFinished = true;
            _mashMarathonController.FlyGuyStoppedSpinning(CharacterType);
            Animator.SetInteger("State", (int)FlyGuyState.Idle);

            HelicopterAS.Stop();
        }

        if (!CanMove && transform.localPosition.y > -0.6f)
        {
            transform.position += Vector3.down * Speed * Time.deltaTime;
        }
    }

    public void StartSpinning()
    {
        CanMove = true;
        Animator.SetInteger("State", (int)FlyGuyState.Spinning);

        HelicopterAS.Play();
    }

    private void RotateGear(bool clockwise, bool extraPush)
    {
        Gear.transform.Rotate(new Vector3(0, -1, 0) * (clockwise ? 1 : -1) * (extraPush ? 15 : 5), Space.Self);
    }
}

public enum FlyGuyState
{
    Idle = 0,
    Spinning = 1
}
