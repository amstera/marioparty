using TMPro;
using UnityEngine;

public class AmountDisplay : MonoBehaviour
{
    public TextMeshPro Text;
    public SpriteRenderer Coin;
    public SpriteRenderer Star;

    public void Display(int amount, AmountType type)
    {
        Text.text = amount >= 0 ? $"+{amount}" : $"{amount}";
        if (type == AmountType.Coin)
        {
            Coin.enabled = true;
        }
        else
        {
            Star.enabled = true;
        }

        Invoke("Hide", 0.75f);
    }

    private void Hide()
    {
        Text.text = string.Empty;
        Coin.enabled = false;
        Star.enabled = false;
    }
}

public enum AmountType
{
    Coin = 0,
    Star = 1
}
