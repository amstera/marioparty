using UnityEngine;

public class Fruit : MonoBehaviour
{
    public Rigidbody Rb;
    public FruitType Type;
    public HoneycombController HoneycombController;
    public ParticleSystem ExplosionPS;

    void Start()
    {
        HoneycombController = FindObjectOfType<HoneycombController>();
    }

    void Update()
    {
        if (transform.localPosition.y < -2.5f)
        {
            HoneycombController.ChangeCharacterTurn(Type == FruitType.Honeycomb);
            var explosion = Instantiate(ExplosionPS, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, 5);
            Destroy(gameObject);
        }
    }

    public void Roll()
    {
        Rb.isKinematic = false;
        Rb.AddForce(-transform.right * (Type == FruitType.Honeycomb ? 7.5f : 4f), ForceMode.Impulse);
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
