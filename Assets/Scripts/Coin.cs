using UnityEngine;

public class Coin : MonoBehaviour
{
    public int Amount = 5;
    public float Speed = 125;

    void Update()
    {
        transform.Rotate(Vector3.up * Speed * Time.deltaTime, Space.Self);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Character")
        {
            var character = collision.collider.GetComponent<Character>();
            character.ChangeCoins(Amount);
        }

        Destroy(gameObject);
    }
}
