using UnityEngine;

public class YellowDice : MonoBehaviour
{
	public float Speed = 80;
    public bool CanRotate = true;
    public int Amount;
    public int FacingAmount;
    public HoneycombController HoneycombController;
    public ParticleSystem Stars;

    public AudioSource DiceRollAS;
    public AudioSource DiceHitAS;

    private float _totalRotated;

    void Start()
    {
        HoneycombController = FindObjectOfType<HoneycombController>();
    }

    void Update()
	{
        if (CanRotate)
        {
            Vector3 rotateAmount = Vector3.left * Speed * Time.deltaTime;
            _totalRotated = (_totalRotated + Mathf.Abs(rotateAmount.x)) % 360;
            transform.Rotate(rotateAmount, Space.Self);
            if (_totalRotated <= 45 || (_totalRotated > 135 && _totalRotated <= 225) || (_totalRotated > 315 && _totalRotated <= 360))
            {
                FacingAmount = 2;
            }
            else
            {
                FacingAmount = 1;
            }
        }
	}

    public void StartRotating()
    {
        CanRotate = true;
        DiceRollAS.Play();
    }

    public void HitSide()
    {
        if (!CanRotate)
        {
            return;
        }

        DiceRollAS.Stop();
        DiceHitAS.Play();

        CanRotate = false;
        Amount = FacingAmount;

        Stars.Play();

        if (Amount == 1)
        {
            transform.localEulerAngles = new Vector3(90, 0, 0);
        }
        else
        {
            transform.localEulerAngles = new Vector3(180, 0, 0);
        }
    }
}
