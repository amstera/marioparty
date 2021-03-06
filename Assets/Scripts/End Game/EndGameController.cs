using System;
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
    public GameObject Star;
    public GameObject Star2;

    public AudioClip FanfareClip;
    public AudioSource EndAS;
    public AudioSource Applause;
    public AudioSource MusicAS;
    public AudioSource ToadetteAS;
    public AudioSource CeremonyAS;
    public AudioSource SuperstarAS;

    private SaveData _saveData;
    private bool _isWinnerShown;

	void Start()
    {
        _saveData = SaveController.Load();
        ToadetteAS.Play();
        Dialog.ShowText("Game's over! Time for the bonus stars!", false, StartBonusStars);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isWinnerShown)
        {
            _isWinnerShown = false;
            WinnerText.text = string.Empty;
            Rankings.ShowRankings(FadeOut, new List<CharacterType> {  CharacterType.Unknown }, true);
        }
    }

    private void StartBonusStars()
    {
        Star.SetActive(false);
        Star2.SetActive(false);

        EndAS.Play();
        Dialog.ShowText("The bonus star for most mini games won goes to...", false, GiveMiniGameBonusStar);
    }

    private void GiveMiniGameBonusStar()
    {
        var miniGameWinner = _saveData.Characters.OrderByDescending(c => c.MiniGamesWon).ThenBy(c => c.Stars).First();
        miniGameWinner.Stars++;

        string dialog = $"{miniGameWinner.Type}! Who won {miniGameWinner.MiniGamesWon} mini games!";

        RevealBonusWinner(miniGameWinner.Type, dialog, PresentSecondBonusStar);
    }

    private void RevealBonusWinner(CharacterType characterType, string dialog, Action callback)
    {
        EndAS.Stop();
        Applause.Play();

        Toadette.SetActive(false);
        Winner = Characters.Find(c => c.Type == characterType);

        Winner.gameObject.SetActive(true);
        Star.SetActive(true);

        Fireworks.Play();
        Winner.Win();

        Dialog.ShowText(dialog, false, callback);
    }

    private void PresentSecondBonusStar()
    {
        Applause.Stop();
        EndAS.Play();
        ToadetteAS.Play();

        Characters.ForEach(c => c.gameObject.SetActive(false));
        Star.SetActive(false);
        Toadette.SetActive(true);

        Dialog.ShowText("The bonus star for most spaces walked goes to...", false, GiveSpacesWalkedBonusStar);
    }

    private void GiveSpacesWalkedBonusStar()
    {
        var spacesWalkedWinner = _saveData.Characters.OrderByDescending(c => c.SpacesWalked).ThenBy(c => c.Stars).First();
        spacesWalkedWinner.Stars++;

        string dialog = $"{spacesWalkedWinner.Type}! Who walked {spacesWalkedWinner.SpacesWalked} spaces!";

        RevealBonusWinner(spacesWalkedWinner.Type, dialog, PresentFinalWinner);
    }

    private void PresentFinalWinner()
    {
        Applause.Stop();
        EndAS.Play();
        ToadetteAS.Play();

        Characters.ForEach(c => c.gameObject.SetActive(false));
        Star.SetActive(false);
        Toadette.SetActive(true);

        Dialog.ShowText("And the winner of Mario Party is...", false, RevealWinner);
    }

    private void RevealWinner()
    {
        CeremonyAS.Stop();
        EndAS.clip = FanfareClip;
        EndAS.Play();
        Applause.Play();
        SuperstarAS.Play();
        MusicAS.PlayDelayed(3);
        _isWinnerShown = true;

        Toadette.SetActive(false);
        CompressedCharacter winnerCharacter = _saveData.Characters.OrderByDescending(c => c.Stars).ThenByDescending(c => c.Coins).First();
        Winner = Characters.Find(c => c.Type == winnerCharacter.Type);
        SaveController.Save(_saveData);

        Winner.gameObject.SetActive(true);
        Star.SetActive(true);
        Star2.SetActive(true);
        Fireworks.Play();
        WinnerText.text = $"{Winner.Type}!";
        Winner.Win();
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
