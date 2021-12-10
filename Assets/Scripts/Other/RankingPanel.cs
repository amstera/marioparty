using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingPanel : MonoBehaviour
{
    public List<Sprite> Thumbnails;
    public TextMeshProUGUI PlaceText;
    public Image ThumbnailImage;
    public Text CoinsText;
    public Text StarsText;
    public GameObject DialogBox;

    public void Reload(int place, Character character, bool showDialogBox)
    {
        ThumbnailImage.sprite = Thumbnails[(int)character.Type - 1];
        CoinsText.text = character.Coins.ToString();
        StarsText.text = character.Stars.ToString();
        PlaceText.text = place == 1 ? "1st" : place == 2 ? "2nd" : place == 3 ? "3rd" : "4th";
        DialogBox.SetActive(showDialogBox);
    }
}
