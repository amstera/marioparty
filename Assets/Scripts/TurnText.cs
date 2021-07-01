using TMPro;
using UnityEngine;

public class TurnText : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void DisplayTurn(int turn)
    {
        Text.text = $"Turn {turn}";
        Invoke("Hide", 1.5f);
    }

    private void Hide()
    {
        Text.text = "";
    }
}
