using UnityEngine;

public class Fruit : MonoBehaviour
{
    public Rigidbody Rb;
    public FruitType Type;
    public HoneycombController HoneycombController;

    void Start()
    {
        HoneycombController = FindObjectOfType<HoneycombController>();
    }

    void Update()
    {
        if (transform.localPosition.y < -2.1f)
        {
            HoneycombController.ChangeCharacterTurn(Type == FruitType.Honeycomb);
            Destroy(gameObject);
        }
    }

    public void Roll()
    {
        Rb.isKinematic = false;
        Rb.AddForce(-transform.right * (Type == FruitType.Honeycomb ? 5f : 4f), ForceMode.Impulse);
    }
}

public enum FruitType
{
    None = 0,
    Honeycomb = 1,
    Pear = 2,
    Orange = 3,
    Apple = 4
}
