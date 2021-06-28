using UnityEngine;

public class Dice : MonoBehaviour
{
    public bool IsRotating = true;
    public float Speed = 125;
    public int ChosenNumber = -1;
    public ParticleSystem PS;

    void Update()
    {
        if (IsRotating)
        {
            transform.Rotate(Vector3.one * Speed * Time.deltaTime, Space.Self);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Character")
        {
            UpdateFace();
        }
    }

    private void UpdateFace()
    {
        if (!IsRotating)
        {
            return;
        }

        IsRotating = false;
        ChosenNumber = Random.Range(1, 7);

        Vector3 newAngle = new Vector3(0, 0, 0);
        switch (ChosenNumber)
        {
            case 1:
                newAngle = new Vector3(180, 0, 180);
                break;
            case 2:
                newAngle = new Vector3(90, 180, 0);
                break;
            case 3:
                newAngle = new Vector3(90, 180, 0);
                break;
            case 4:
                newAngle = new Vector3(180, -90, 180);
                break;
            case 5:
                newAngle = new Vector3(90, 0, 0);
                break;
        }

        PS.Play();
        transform.localEulerAngles = newAngle;
    }
}
