using UnityEngine;

public class MashCharacter : MonoBehaviour
{
    public Animator Animator;
    public CharacterType Type;
    public bool IsPlayer;

    public AudioSource VictoryAS;
    public AudioSource LoseAS;

    void Start()
    {
        Animator.SetInteger("State", (int)CharacterState.Holding);
    }

    public void Idle()
    {
        Animator.SetInteger("State", (int)CharacterState.Idle);
    }

    public void Win()
    {
        Animator.SetInteger("State", (int)CharacterState.Idle);
        Animator.SetTrigger("Victory");
        VictoryAS.Play();
    }

    public void Lose()
    {
        LoseAS.Play();
    }
}
