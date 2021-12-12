using UnityEngine;

public class FlagCharacter : MonoBehaviour
{
    public Animator Animator;
    public GameObject FlagA;
    public GameObject FlagB;

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

    void Update()
    {
        if (Eliminated)
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
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    Flag = FlagType.B;
                    FlagB.SetActive(true);
                }
            }
            else
            {
                if (Random.Range(0, 4) == 1)
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
    }

    public void MakeChangeFlag()
    {
        if (IsPlayer)
        {
            CanChangeFlag = true;
        }
        else
        {
            Invoke("WaitToChangeFlag", Random.Range(0.35f, Mathf.Min(1.5f, TimeBetweenTurns)));
        }
    }

    public void Eliminate()
    {
        Eliminated = true;
        LoseAS.Play();
        Destroy(gameObject, 2f);
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
}
