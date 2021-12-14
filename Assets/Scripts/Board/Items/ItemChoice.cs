using UnityEngine;
using UnityEngine.UI;

public class ItemChoice : MonoBehaviour
{
    public Image ItemImage;
    public Image Selection;
    public Text ItemName;
    public ItemType Type;

    public bool IsSelected;

    public void Set(string name, Sprite sprite, ItemType type)
    {
        ItemName.text = name;
        ItemImage.sprite = sprite;
        Type = type;
    }

    public void Select(bool isSelected)
    {
        IsSelected = isSelected;
        Selection.enabled = isSelected;
    }
}
