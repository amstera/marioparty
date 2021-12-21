using UnityEngine;
using UnityEngine.UI;

public class MiniGameOption : MonoBehaviour
{
    public Color DefaultColor;
    public Color SelectedColor;
    public string MiniGameSceneName;
    public Image Button;

    public void Select(bool isSelected)
    {
        Button.color = isSelected ? SelectedColor : DefaultColor;
    }
}
