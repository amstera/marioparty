using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AirHockeyController : MonoBehaviour
{
    public List<HockeyCharacter> Characters;
    public SpecialText Text;
    public ParticleSystem Fireworks;
    public FadePanel FadePanel;
    public List<CharacterType> Winners;

    public Lights RedLights;
    public Lights BlueLights;
    public Shell Shell;

    public AudioSource MiniGameAS;
    public AudioSource MusicAS;
    public AudioSource GoalHornAS;

    public List<AudioClip> MiniGameSounds;

    private SaveData _saveData;

    void Start()
    {
        _saveData = SaveController.Load();
        SetMainPlayer();
        Text.Show("Ready...", 1.5f);
        Invoke("ShowGoText", 1.5f);
    }

    public void AddScore(TeamColor color)
    {
        GoalHornAS.Play();

        if (color == TeamColor.Blue)
        {
            BlueLights.AddScore();
        }
        else
        {
            RedLights.AddScore();
        }

        Characters.FindAll(c => c.Color == color).ForEach(c => c.Win());

        if (RedLights.Score == 3)
        {
            ShowWinner(TeamColor.Red);
        }
        else if (BlueLights.Score == 3)
        {
            ShowWinner(TeamColor.Blue);
        }
        else
        {
            Invoke("AddShell", 0.5f);
        }
    }

    private void ShowWinner(TeamColor color)
    {
        Characters.ForEach(c => c.CanMove = false);

        foreach (var character in Characters)
        {
            if (character.Color == color)
            {
                Winners.Add(character.Type);
            }
        }

        MusicAS.Stop();
        MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Win];
        MiniGameAS.Play();

        Text.Show($"{Winners[0]} & {Winners[1]} Win!", 2f);
        Fireworks.Play();
        Invoke("FadeOut", 2f);
    }

    private void ShowGoText()
    {
        MiniGameAS.clip = MiniGameSounds[(int)MiniGameSoundType.Go];
        MiniGameAS.Play();
        Text.Show("Go!", 0.5f);

        Characters.ForEach(c => c.CanMove = true);
        AddShell();
    }

    private void AddShell()
    {
        Instantiate(Shell, new Vector3(0, -6.7f, -2.5f), Shell.transform.rotation);
    }

    private void FadeOut()
    {
        FadePanel.FadeOut();
        Invoke("ReturnGame", 1f);
    }

    private void ReturnGame()
    {
        if (_saveData != null)
        {
            _saveData.LastWinningCharacters = Winners;
            _saveData.LastMiniGame = "Air Hockey";
            if (Winners.Count > 0)
            {
                foreach (var character in _saveData.Characters)
                {
                    if (Winners.Any(w => w == character.Type))
                    {
                        character.Coins += 10;
                        character.MiniGamesWon++;
                    }
                }
            }

            SaveController.Save(_saveData);
        }

        SceneManager.LoadScene("Game");
    }

    private void SetMainPlayer()
    {
        if (_saveData != null)
        {
            foreach (var character in Characters)
            {
                var matchingCharacter = _saveData.Characters.Find(c => c.Type == character.Type);
                character.IsPlayer = matchingCharacter.IsPlayer;
            }
        }
    }
}

public enum TeamColor
{
    None = 0,
    Blue = 1,
    Red = 2
}
