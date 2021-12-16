using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AirHockeyController : MonoBehaviour
{
    public List<HockeyCharacter> Characters;
    public SpecialText Text;
    public ParticleSystem Fireworks;
    public FadePanel FadePanel;
    public CharacterType Winner;

    public Lights RedLights;
    public Lights BlueLights;
    public Shell Shell;

    public AudioSource MiniGameAS;
    public AudioSource MusicAS;
    public AudioSource GoalHornAS;

    public List<AudioClip> MiniGameSounds;

    private SaveData _saveData;
    private bool _isDraw;

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
            AddShell();
        }
    }

    private void ShowWinner(TeamColor color)
    {
        Characters.ForEach(c => c.CanMove = false);

        //do stuff
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
            _saveData.LastWinningCharacter = _isDraw ? CharacterType.Unknown : Winner;
            _saveData.LastMiniGame = "Shy Guy";
            if (Winner != CharacterType.Unknown)
            {
                var winningCharacter = _saveData.Characters.Find(c => c.Type == Winner);
                winningCharacter.Coins += 10;
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
