using TMPro;
using UnityEngine;

public class TurnText : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public TextMeshProUGUI TurnsLeft;

    public void DisplayTurn(int turn, int totalTurns)
    {
        Text.text = $"Turn {turn}";
        int turnsLeft = totalTurns - turn + 1;
        TurnsLeft.text = turnsLeft == 1 ? "Final turn" : $"{turnsLeft} turn{(turnsLeft == 1 ? "" : "s")} left";
        Invoke("Hide", 1.5f);
    }

    private void Hide()
    {
        Text.text = "";
        TurnsLeft.text = "";
    }
}
