using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AirHockeyController : MonoBehaviour
{
    public List<HockeyCharacter> Characters;
    public List<HockeyCharacter> FlippedCharacters;
    public SpecialText Text;
    public ParticleSystem Fireworks;
    public FadePanel FadePanel;
    public List<CharacterType> Winners;

    public Lights RedLights;
    public Lights BlueLights;
    public Shell Shell;

    public Material RedColor;
    public Material BlueColor;

    public AudioSource MiniGameAS;
    public AudioSource MusicAS;
    public AudioSource GoalHornAS;

    public List<AudioClip> MiniGameSounds;

    private SaveData _saveData;

    void Awake()
    {
        SetCharacterPositions();
    }

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

    private void SetCharacterPositions()
    {
        var positions = Characters.Select(c => c.transform.position).ToList();
        positions.Shuffle();
        for (int i = 0; i < Characters.Count; i++)
        {
            var character = Characters[i];
            var flipDiff = FlippedCharacters[i].transform.position - Characters[i].transform.position; //hacky
            character.transform.position = new Vector3(positions[i].x, character.transform.position.y, character.transform.position.z);
            var boardRenderer = character.transform.Find("Block").GetComponent<Renderer>();
            var mats = boardRenderer.materials;
            if (character.transform.position.x < -1)
            {
                character.Color = TeamColor.Red;
                mats[0] = RedColor;
            }
            else
            {
                character.Color = TeamColor.Blue;
                character.transform.localRotation = FlippedCharacters[i].transform.localRotation;
                character.transform.position += flipDiff;
                mats[0] = BlueColor;
            }
            boardRenderer.materials = mats;
        }
    }
}

public enum TeamColor
{
    None = 0,
    Blue = 1,
    Red = 2
}
