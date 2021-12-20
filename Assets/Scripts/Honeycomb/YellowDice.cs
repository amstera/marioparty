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
            UpdateFacingAmount();
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

        UpdateFacingAmount();

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

    private void UpdateFacingAmount()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, transform.position - Camera.main.transform.position, out hit))
        {
            if (hit.collider.name == "Side 1" || hit.collider.name == "Side 3")
            {
                FacingAmount = 1;
            }
            else if (hit.collider.name == "Side 2" || hit.collider.name == "Side 4")
            {
                FacingAmount = 2;
            }
        }
    }
}
