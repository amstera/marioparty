using UnityEngine;

public class FlagCharacter : MonoBehaviour
{
    public Animator Animator;
    public GameObject FlagA;
    public GameObject FlagB;
    public GameObject FlagNeutral;
    public ShyGuy ShyGuy;

    public FlagType Flag;
    public FlagType CorrectFlag;

    public CharacterType Type;
    public bool IsPlayer;
    public bool CanChangeFlag;
    public float TimeBetweenTurns;
    public float Speed = 5;
    public bool Eliminated;

    public AudioSource VictoryAS;
    public AudioSource LoseAS;

    private bool _startMoving;

    void Update()
    {
        if (_startMoving)
        {
            transform.position += Vector3.forward * Speed * Time.deltaTime;
            return;
        }

        if (Flag == FlagType.None && CanChangeFlag)
        {
            if (IsPlayer)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    Flag = FlagType.A;
                    FlagA.SetActive(true);
                    FlagNeutral.SetActive(false);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    Flag = FlagType.B;
                    FlagB.SetActive(true);
                    FlagNeutral.SetActive(false);
                }
            }
            else if (!ShyGuy.ShowingTwoFlags)
            {
                FlagNeutral.SetActive(false);
                if (Random.Range(0, 7) == 1)
                {
                    Flag = CorrectFlag == FlagType.A ? FlagType.B : FlagType.A;
                }
                else
                {
                    Flag = CorrectFlag;
                }

                if (Flag == FlagType.A)
                {
                    FlagA.SetActive(true);
                }
                else
                {
                    FlagB.SetActive(true);
                }
            }
        }
    }

    public void HideFlag()
    {
        Flag = FlagType.None;
        FlagA.SetActive(false);
        FlagB.SetActive(false);
        FlagNeutral.SetActive(true);
    }

    public void MakeChangeFlag()
    {
        if (IsPlayer)
        {
            CanChangeFlag = true;
        }
        else
        {
            Invoke("WaitToChangeFlag", Random.Range(0.35f, Mathf.Min(1f, TimeBetweenTurns)));
        }
    }

    public void Eliminate()
    {
        Invoke("StartMoving", 0.5f);
        Eliminated = true;
        LoseAS.Play();
        Destroy(gameObject, 3f);
    }

    public void Win()
    {
        Animator.SetInteger("State", (int)CharacterState.Idle);
        Animator.SetTrigger("Victory");
        VictoryAS.Play();
    }

    private void WaitToChangeFlag()
    {
        CanChangeFlag = true;
    }

    private void StartMoving()
    {
        _startMoving = true;
    }
}
