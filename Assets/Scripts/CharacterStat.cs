using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStat : MonoBehaviour
{
    public List<Sprite> Thumbnails;
    public Image ThumbnailImage;
    public Text CoinsText;
    public Text StarsText;
    public Text PlaceText;

    public void Reload(Character character, int place)
    {
        ThumbnailImage.sprite = Thumbnails[(int)character.Type - 1];
        CoinsText.text = character.Coins.ToString();
        StarsText.text = character.Stars.ToString();
        PlaceText.text = place == 0 ? "1st" : place == 1 ? "2nd" : place == 2 ? "3rd" : "4th";
    } 
}