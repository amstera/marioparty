using UnityEngine;

public class HiddenBlock : MonoBehaviour
{
    public GameController GameController;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Character"))
        {
            GameController.AddStar();
            Destroy(gameObject);
        }
    }
}
