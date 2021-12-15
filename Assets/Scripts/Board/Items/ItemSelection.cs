using UnityEngine;
using UnityEngine.UI;

public class ItemSelection : MonoBehaviour
{
    public Text ItemNameText;
    public Text ItemCostText;
    public RawImage Selected;

    public Sprite ItemImage;
    public ItemType Type;
    public int Cost;
    public string Description;

    public bool IsSelected;

    void Awake()
    {
        ItemCostText.text = Cost.ToString();
    }

    public void Select(bool select)
    {
        IsSelected = select;
        Selected.enabled = select;
    }
}

public enum ItemType
{
    None = 0,
    Mushroom = 1,
    DoubleDice = 2,
    WarpBlock = 3,
    GoldenPipe = 4
}
