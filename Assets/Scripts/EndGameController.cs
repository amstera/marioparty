using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
	public Dialog Dialog;
    public GameObject Toadette;
    public List<CharacterChoose> Characters;
    public ParticleSystem Fireworks;
    public CharacterChoose Winner;
    public TextMeshProUGUI WinnerText;
    public Rankings Rankings;
    public FadePanel FadePanel;

    private SaveData _saveData;
    private bool _isWinnerShown;

	void Start()
    {
        _saveData = SaveController.Load();
        Dialog.ShowText("And the winner of Mario Party is...", RevealWinner);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isWinnerShown)
        {
            _isWinnerShown = false;
            WinnerText.text = string.Empty;
            Rankings.ShowRankings(FadeOut, CharacterType.Unknown, true);
        }
    }

    private void RevealWinner()
    {
        _isWinnerShown = true;

        Toadette.SetActive(false);
        CompressedCharacter winnerCharacter = _saveData.Characters.OrderByDescending(c => c.Stars).ThenByDescending(c => c.Coins).First();
        Winner = Characters.Find(c => c.Type == winnerCharacter.Type);

        Winner.gameObject.SetActive(true);
        Fireworks.Play();
        WinnerText.text = $"{Winner.Type}!";
        Winner.GetComponentInChildren<Animator>().SetTrigger("Victory");
    }

    private void FadeOut()
    {
        FadePanel.FadeOut();
        Invoke("RestartGame", 1f);
    }

    private void RestartGame()
    {
        SceneManager.LoadSceneAsync("Character Chooser");
    }
}