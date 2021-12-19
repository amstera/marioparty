using UnityEngine;

public class YellowDice : MonoBehaviour
{
	public float Speed = 125;
    public bool CanRotate = true;
    public int Amount;
    public HoneycombController HoneycombController;

    void Start()
    {
        HoneycombController = FindObjectOfType<HoneycombController>();
    }

    void Update()
	{
        if (CanRotate)
        {
            transform.Rotate(Vector3.left * Speed * Time.deltaTime, Space.Self);
        }
	}

    public void HitSide(int amount)
    {
        if (!CanRotate)
        {
            return;
        }

        CanRotate = false;
        Amount = amount;
        if (amount == 1)
        {
            transform.localEulerAngles = new Vector3(90, 0, 0);
        }
        else
        {
            transform.localEulerAngles = new Vector3(180, 0, 0);
        }
    }
}
