using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStat : MonoBehaviour
{
    public List<Sprite> Thumbnails;
    public List<Image> Items;
    public Image ThumbnailImage;
    public Text CoinsText;
    public Text StarsText;
    public Text PlaceText;

    public void Reload(Character character, ItemsPanel itemsPanel, int place)
    {
        ThumbnailImage.sprite = Thumbnails[(int)character.Type - 1];
        CoinsText.text = character.Coins.ToString();
        StarsText.text = character.Stars.ToString();
        PlaceText.text = place == 1 ? "1st" : place == 2 ? "2nd" : place == 3 ? "3rd" : "4th";

        for (int i = 0; i < Items.Count; i++)
        {
            if (character.Items.Count >= i + 1)
            {
                Items[i].sprite = itemsPanel.ItemSelections.Find(item => item.Type == character.Items[i]).ItemImage;
                Items[i].enabled = true;
            }
            else
            {
                Items[i].enabled = false;
            }
        }
    } 
}