using TMPro;
using UnityEngine;

public class StarText : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public GameObject Fireworks;

    public void Show(Vector3 pos)
    {
        Text.enabled = true;
        GameObject fireworks = Instantiate(Fireworks, pos, Quaternion.identity);
        Destroy(fireworks, 5);

        Invoke("Hide", 1.5f);
    }

    private void Hide()
    {
        Text.enabled = false;
    }
}
