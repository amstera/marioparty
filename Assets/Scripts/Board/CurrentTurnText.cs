using TMPro;
using UnityEngine;

public class CurrentTurnText : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void Display(int turn, int maxTurns)
    {
        Text.text = $"{turn}/{maxTurns}";
    }
}
