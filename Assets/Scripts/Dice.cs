using TMPro;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public bool IsRotating;
    public float Speed = 125;
    public int ChosenNumber = -1;
    public ParticleSystem Stars;
    public ParticleSystem Explosion;
    public TextMeshPro Text;
    public GameObject Sides;
    public Character Character;

    void Start()
    {
        Text.transform.SetParent(null);
    }

    void Update()
    {
        if (IsRotating)
        {
            transform.Rotate(Vector3.one * Speed * Time.deltaTime, Space.Self);
        }
    }

    void OnEnable()
    {
        IsRotating = true;
        ShowHideSides(true);
        Text.text = string.Empty;
    }

    void OnDisable()
    {
        Text.text = string.Empty;
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
                newAngle = new Vector3(0, -90, 0);
                break;
            case 4:
                newAngle = new Vector3(180, -90, 180);
                break;
            case 5:
                newAngle = new Vector3(90, 0, 0);
                break;
        }

        Stars.Play();
        transform.localEulerAngles = newAngle;
        Character.Roll = ChosenNumber;

        Invoke("RevealNumber", 0.55f);
    }

    private void RevealNumber()
    {
        Explosion.Play();
        ShowHideSides(false);
        Text.transform.position = transform.position;
        Text.text = ChosenNumber.ToString();
    }

    private void ShowHideSides(bool showSides)
    {
        MeshRenderer[] meshRenderers = Sides.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = showSides;
        }
    }
}
