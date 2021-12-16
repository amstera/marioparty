using UnityEngine;

public class HockeyCharacter : MonoBehaviour
{
    public Animator Animator;
    public CharacterType Type;

    public bool IsPlayer;
    public float Speed = 5.5f;
    public bool CanMove;

    public AudioSource VictoryAS;
    public AudioSource LoseAS;

    private Shell _shell;

    void Update()
    {
        if (!CanMove)
        {
            return;
        }

        if (IsPlayer)
        {
            if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && transform.position.z > -6f)
            {
                transform.position += Vector3.back * Speed * Time.deltaTime;
            }
            else if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && transform.position.z < 1.5f)
            {
                transform.position += Vector3.forward * Speed * Time.deltaTime;
            }
        }
        else
        {
            if (_shell == null)
            {
                _shell = FindObjectOfType<Shell>();
            }

            if (_shell != null)
            {
                float buffer = 0.75f;
                if (transform.position.z < _shell.transform.position.z - buffer)
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.forward * Speed, Time.deltaTime);
                }
                else if (transform.position.z > _shell.transform.position.z + buffer)
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.back * Speed, Time.deltaTime);
                }
            }
        }
    }

    public void Win()
    {
        Animator.SetInteger("State", (int)CharacterState.Idle);
        Animator.SetTrigger("Victory");
        VictoryAS.Play();
    }
}
