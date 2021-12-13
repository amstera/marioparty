using TMPro;
using UnityEngine;

public class AmountDisplay : MonoBehaviour
{
    public TextMeshPro Text;
    public SpriteRenderer Coin;
    public SpriteRenderer Star;
    public SpriteRenderer CustomImage;
    public VertexGradient PositiveGradient;
    public VertexGradient NegativeGradient;

    public void Display(int amount, AmountType type, Sprite customImage, Vector3 pos)
    {
        Hide();
        transform.position = pos + Vector3.up * 2;
        Text.text = amount >= 0 ? $"+{amount}" : $"{amount}";
        if (type == AmountType.Coin)
        {
            Coin.enabled = true;
        }
        else if (type == AmountType.Star)
        {
            Star.enabled = true;
        }
        else if (type == AmountType.Item)
        {
            CustomImage.enabled = true;
            CustomImage.sprite = customImage;
        }

        if (amount < 0)
        {
            Text.colorGradient = NegativeGradient;
        }
        else
        {
            Text.colorGradient = PositiveGradient;
        }

        Invoke("Hide", 0.75f);
    }

    private void Hide()
    {
        Text.text = string.Empty;
        Coin.enabled = false;
        Star.enabled = false;
        CustomImage.enabled = false;
    }
}

public enum AmountType
{
    Coin = 0,
    Star = 1,
    Item = 2
}
