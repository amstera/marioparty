using TMPro;
using UnityEngine;

public class SpacesText : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void Show(string text)
    {
        Text.text = text;
    }

    public void Hide()
    {
        Text.text = string.Empty;
    }
}
