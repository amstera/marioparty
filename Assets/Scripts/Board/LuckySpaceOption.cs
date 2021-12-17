using UnityEngine;
using UnityEngine.UI;

public class LuckySpaceOption : MonoBehaviour
{
    public Color DefaultColor;
    public Color SelectedColor;
    public LuckySpaceOptionType Type;
    public Image Button;

    public void Select(bool isSelected)
    {
        Button.color = isSelected ? SelectedColor : DefaultColor;
    }
}

public enum LuckySpaceOptionType
{
    None = 0,
    Coins5 = 1,
    Coins10 = 2,
    Coins15 = 3,
    Mushroom = 4,
    DoubleDice = 5,
    GoldenPipe = 6
}