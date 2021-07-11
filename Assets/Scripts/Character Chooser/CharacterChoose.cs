using UnityEngine;

public class CharacterChoose : MonoBehaviour
{
    public CharacterType Type;
    public bool IsPlayer;

    public Animator Animator;
    public AudioSource VictoryAS;

    public void Win()
    {
        Animator.SetTrigger("Victory");
        VictoryAS.Play();
    }
}
