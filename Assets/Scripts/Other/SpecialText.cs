using TMPro;
using UnityEngine;

public class SpecialText : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void Show(string text, float time)
    {
        Text.text = text;
        Invoke("Hide", time);
    }

    private void Hide()
    {
        Text.text = "";
    }
}
