using UnityEngine;

public class Character : MonoBehaviour
{
    public Animator Animator;
    public Rigidbody Rb;
    public bool IsPlayer;
    public bool CanJump;

    void FixedUpdate()
    {
        if (IsPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    public void Jump()
    {
        if (!CanJump)
        {
            return;
        }

        Animator.SetInteger("State", (int)CharacterState.Jump);
        Rb.AddForce(new Vector3(0, 6, 0), ForceMode.Impulse);
        CanJump = false;

        Invoke("Land", 0.5f);
    }

    private void Land()
    {
        Animator.SetInteger("State", (int)CharacterState.Idle);
    }
}

public enum CharacterState
{
    Idle = 0,
    Jump = 1,
    Walk = 2
}
