using UnityEngine;

public class EndGameController : MonoBehaviour
{
	public Dialog Dialog;
    public GameObject Toadette;

	void Start()
    {
        Dialog.ShowText("And the winner of Mario Party is...", RevealWinner);
    }

    private void RevealWinner()
    {
        Toadette.SetActive(false);
        //show winner's model, add fireworks, and show fancy text displaying name
    }
}
