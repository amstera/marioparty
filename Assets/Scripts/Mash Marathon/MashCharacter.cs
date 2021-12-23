using UnityEngine;

public class MashCharacter : MonoBehaviour
{
    public Animator Animator;
    public AudioSource VictoryAS;
    public AudioSource LoseAS;

    void Start()
    {
        Animator.SetInteger("State", (int)CharacterState.Holding);
    }

    public void Win()
    {
        Animator.SetInteger("State", (int)CharacterState.Idle);
        Animator.SetTrigger("Victory");
        VictoryAS.Play();
    }
}
